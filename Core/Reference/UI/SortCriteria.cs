﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Core.Model.Parameters;

namespace Core.Reference
{
	public enum SortCriteria
	{
		[EnumMember(Value = "popular")]
		[Display(Name = "Popular", ShortName = "Popular")]
		Popular,

		[EnumMember(Value = "newest")]
		[Display(Name = "Newest", ShortName = "New")]
		Newest,

		[EnumMember(Value = "featured")]
		[Display(Name = "Featured", ShortName = "Featured")]
		Featured,

		[EnumMember(Value = "PriceFromLow")]
		[Display(Name = "Price (Low-High)", ShortName = "Price Low")]
		PriceFromLow,

		[EnumMember(Value = "PriceFromHigh")]
		[Display(Name = "Price (High-Low)", ShortName = "Price High")]
		PriceFromHigh
	}
}