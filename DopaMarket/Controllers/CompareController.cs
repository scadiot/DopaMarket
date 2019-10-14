using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DopaMarket.Models;
using DopaMarket.ViewModels;

namespace DopaMarket.Controllers
{
    public class CompareController : BaseController
    {
        ApplicationDbContext _context;

        public CompareController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var items = (from i in _context.Items
                         //join sg in _context.CompareGroups on i.CompareGroupId equals sg.Id
                         //where sg.LinkName == "processeur"
                         join ib in _context.ItemCompares on i.Id equals ib.ItemId
                         where ib.SessionId == Session.SessionID
                         select i).ToArray();

            CompareViewModel CompareViewModel = new CompareViewModel();
            if (items.Count() > 0)
            {
                var compareGroupId = items.First().CompareGroupId;
                var compareGroup = _context.CompareGroups.Single(c => c.Id == compareGroupId);
                var specifications = (from s in _context.Specifications
                                      join cs in _context.CompareGroupSpecifications on s.Id equals cs.SpecificationId
                                      where cs.CompareGroupId == compareGroup.Id
                                      select s).ToArray();

                var groupIds = specifications.Select(s => s.SpecificationGroupId).Distinct().ToArray();
                var specificationGroups = (from g in _context.SpecificationGroups
                                           where groupIds.Contains(g.Id)
                                           select g).ToArray();

                var itemIds = items.Select(i => i.Id).ToArray();

                var itemSpecifications = (from ispec in _context.ItemSpecifications
                                          where itemIds.Contains(ispec.ItemId)
                                          select ispec).ToArray();


                var columns = new List<ColumCompareViewModel>();
                foreach (var item in items)
                {
                    var column = new ColumCompareViewModel();
                    column.Item = item;
                    columns.Add(column); 
                }
                CompareViewModel.Columns = columns;

                var lines = new List<LineCompareViewModel>();
                foreach(var specificationGroup in specificationGroups)
                {
                    var groupLine = new LineCompareViewModel();
                    groupLine.Type = LineCompareViewModelType.Group;
                    groupLine.CompareGroup = specificationGroup;
                    var groupCells = new List<CellCompareViewModel>();
                    foreach (var item in items)
                    {
                        var groupCell = new CellCompareViewModel();
                        groupCell.Value = "";
                        groupCells.Add(groupCell);
                    }
                    groupLine.Cells = groupCells;
                    lines.Add(groupLine);

                    foreach (var specification in specifications.Where(s => s.SpecificationGroupId == specificationGroup.Id))
                    {
                        var line = new LineCompareViewModel();
                        line.Type = LineCompareViewModelType.Specification;
                        line.Specification = specification;
                        var cells = new List<CellCompareViewModel>();
                        foreach (var item in items)
                        {
                            var cell = new CellCompareViewModel();
                            var itemSpecification = itemSpecifications.SingleOrDefault(ispec => ispec.ItemId == item.Id && ispec.SpecificationId == specification.Id);
                            cell.Value = GetCellValue(specification, itemSpecification);
                            cells.Add(cell);
                        }
                        line.Cells = cells;
                        lines.Add(line);
                    }
                }
                CompareViewModel.Lines = lines;
            }
            else
            {

            }

            return View("Index", CompareViewModel);
        }

        string GetCellValue(Specification specification, ItemSpecification itemSpecification)
        {
            if(itemSpecification == null)
            {
                return "NC";
            }
            string value = "";
            switch(specification.Type)
            {
                case SpecificationType.Boolean:
                    value = itemSpecification.BooleanValue.ToString();
                    break;
                case SpecificationType.Interger:
                    value = itemSpecification.IntegerValue.ToString();
                    break;
                case SpecificationType.String:
                    value = itemSpecification.StringValue.ToString();
                    break;
                case SpecificationType.Decimal:
                    value = itemSpecification.DecimalValue.ToString();
                    break;
            }
            value += " " + specification.Unity;
            return value;
        }

        public ActionResult AddItem(int id)
        {
            if (_context.ItemCompares.Any(ib => ib.SessionId == Session.SessionID && ib.ItemId == id))
            {
                return Json(new { result = "error", message = "already exist" }, JsonRequestBehavior.AllowGet);
            }

            var itemCompare = new ItemCompare();
            itemCompare.ItemId = id;
            itemCompare.SessionId = Session.SessionID;
            _context.ItemCompares.Add(itemCompare);
            _context.SaveChanges();

            return Json(new { result = "added" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveItem(int id)
        {
            var itemCompare = _context.ItemCompares.SingleOrDefault(ib => ib.SessionId == Session.SessionID && ib.ItemId == id);
            if(itemCompare != null)
            {
                _context.ItemCompares.Remove(itemCompare);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Compare");
        }

        public ActionResult ListItems()
        {
            var items = (from i in _context.Items
                         join ib in _context.ItemCompares on i.Id equals ib.ItemId
                         where ib.SessionId == Session.SessionID
                         select i).ToArray();

            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}