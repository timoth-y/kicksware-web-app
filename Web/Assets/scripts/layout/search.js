﻿import {gsap, TweenMax, Elastic} from "gsap/dist/gsap"

function searchPanelInit() {
	let icon = $(".search");
	let panel = $(".search-panel");
	if (icon.length && panel.length) {
		let header = $("header");
		let dismiss = $(".search-panel .close-button");
		let searchInput = $(".search-panel input");

		icon.on("click", function () {
			panel.toggleClass("active");
			setTimeout(function () {
				searchInput.focus();
				$("body").css("overflow", "hidden");
			}, 1200);
		});
		dismiss.on("click", function () {
			$("body").css("overflow", "auto");
			panel.toggleClass("active");
		});
		window.addEventListener("click", function (event) {
			if (panel.hasClass("active") && !isDescendant(header[0], event.target)) {
				panel.toggleClass("active");
			}
		});
	}
}

function mainSearchInit() {
	let searchForm = $(".search-panel form[method=get]");
	let searchInput = searchForm.find("input[type=search]");
	let action = searchForm.attr("action")

	searchInput.on("input", function () {
		submitAutoSearch(action, this.value);
	})
	$(window).keypress(function(event) {
		if (event.keyCode === 13) {
			event.preventDefault();
			if ($(event.target).is(searchInput)) {
				submitAutoSearch(action, event.target.value);
			}
			return false;
		}
	});
}


function submitAutoSearch(actionUrl, prefix) {
	$.get(`${actionUrl}?prefix=${prefix}`, function (response) {
		if (!response["success"]) {
			return;
		}
		$("header .search-results").html(response["content"]);
		loadingSearch($("header .result-cell"));
		searchFavoriteInit();
	})
}


function loadingSearch(items){
	TweenMax.staggerFrom(items, 1, {
		scale: 0.6,
		opacity: 0,
		delay: .1,
		ease: Elastic.easeOut,
		force3D: true,
		clearProps: "all"
	}, 0.05);
}

function searchFavoriteInit(){
	$("header .search-results .result-cell .favorite input[type=checkbox]").click(function () {
		let id = $(this).closest(".result-cell").attr("id")
		let checked = $(this).is(":checked");
		$.get(`/shop/${checked ? "like" : "unlike"}/${id}`);
		$(this).parent().toggleClass("liked")
	})
}

$(document).ready(function () {
	"use strict";

	searchPanelInit();

	mainSearchInit();

	searchFavoriteInit();
});