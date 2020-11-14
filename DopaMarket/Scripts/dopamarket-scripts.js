/*
 * Unishop | Universal E-Commerce Template
 * Copyright 2018 rokaux
 * Theme Custom Scripts
 */

/*global jQuery, iziToast, noUiSlider*/

jQuery(document).ready(function ($) {
    'use strict';

    var cartDropdownItemsContainer = $("#cart-dropdown-items-container");
    var cartDropdownItem = $("#cart-dropdown-item");
    cartDropdownItem.remove();

    function GetItemsInCart() {
        $.ajax({
            url: "/Cart/ListItems",
        }).done(function (response) {
            if (response.error) {
                return;
            }
            $(".topbar-cart-count-label").each(function (index) {
                $(this).text(response.Items.length);
            })
            var total = 0;
            for (var i = response.Items.length - 1; i >= 0; i--) {
                var item = response.Items[i];
                var newItem = cartDropdownItem.clone();
                var link = newItem.find(".cart-dropdown-item-link");
                var image = newItem.find(".cart-dropdown-item-image");
                var info = newItem.find(".cart-dropdown-item-info");
                link.text(item.Item.Name);
                link.attr("href", "/Item/" + item.Item.LinkName);
                image.attr("src", "/img/items/" + item.Item.LinkName + "_0.jpg");
                info.text(item.Quantity + " x " + item.Item.CurrentPrice + " €");
                newItem.prependTo(cartDropdownItemsContainer);
                total += item.Quantity * item.Item.CurrentPrice;
            }
            $(".cart-dropdown-total").each(function (index) {
                $(this).text((Math.round(total * 100) / 100)+ " €");
            })
        });
    }

    function GetItemsCountInCompare() {
        $.ajax({
            url: "/Compare/ListItems",
        }).done(function (response) {
            if (response.error) {
                return;
            }
            $(".topbar-compare-count-label").each(function (index) {
                $(this).text(response.length);
            })
        });
    }

    GetItemsInCart();
    GetItemsCountInCompare();
});

function ShippingAddressFormToggle() {
    $("#shipping-address-form").toggle();
}