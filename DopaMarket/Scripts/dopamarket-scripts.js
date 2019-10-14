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
            for (var i = response.Items.length - 1; i >= 0; i--) {
                var item = response.Items[i];
                var newItem = cartDropdownItem.clone();
                var link = newItem.find(".cart-dropdown-item-link");
                link.text(item.Item.Name);
                newItem.prependTo(cartDropdownItemsContainer);
            }
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