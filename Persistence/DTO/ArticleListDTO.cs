using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Persistence.DTO
{
    public class ArticleListDTO
    {
        [Required] public int Limit { get; set; }
        [Required] public int Page { get; set; }
        [Required] public int Count { get; set; }
        [Required] public int PageCount { get; set; }
        [Required] public IEnumerable<ArticleListElemDTO> Articles { get; set; }
    }
}
