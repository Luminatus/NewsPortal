using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace NewsPortal.Website.Models
{
    public class SearchModel
    {
        [FromQuery(Name="page")]
        public int Page { get; set; }

        public int Limit { get; set; }

        [Display(Name = "Keresés")]
        [FromQuery(Name = "search")]
        public string Search { get; set; }

        public int PageCount { get; set; }

        [Display(Name = "Kezdő dátum")]
        [DataType(DataType.Date)]
        [FromQuery(Name = "date_start")]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
        public DateTime? DateStart { get; set; }

        [Display(Name = "Végdátum")]
        [DataType(DataType.Date)]
        [FromQuery(Name = "date_end")]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
        public DateTime? DateEnd { get; set; }

        [Display(Name = "Keresés címekben")]
        [FromQuery(Name="search_title")]
        public bool SearchTitle { get; set; }

        [Display(Name = "Keresés bevezetőkben")]
        [FromQuery(Name = "search_lead")]
        public bool SearchLead { get; set; }

        [Display(Name = "Keresés tartalomban")]
        [FromQuery(Name = "search_content")]
        public bool SearchContent { get; set; }
    }
}
