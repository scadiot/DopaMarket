using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class ColumCompareViewModel
    {
        public Models.Item Item { get; set; }
    }

    public enum LineCompareViewModelType
    {
        Group, Specification
    }
    public class LineCompareViewModel
    {
        public Models.Specification Specification { get; set; }
        public Models.SpecificationGroup CompareGroup { get; set; }
        public IEnumerable<CellCompareViewModel> Cells { get; set; }

        public LineCompareViewModelType Type { get; set; }
    }

    public class CellCompareViewModel
    {
        public string Value { get; set; }
    }

    public class CompareViewModel
    {
        public IEnumerable<ColumCompareViewModel> Columns { get; set; }
        public IEnumerable<LineCompareViewModel> Lines { get; set; }
    }
}