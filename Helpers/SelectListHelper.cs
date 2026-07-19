using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using BackLog.Models;
using System.Runtime.Serialization;

namespace BackLog.Helpers
{
    public static class SelectListHelper
    {
        public static List<SelectListItem> GetYearList(int minimumYear = 1900, string? allLabel = null)
        {
            var years = Enumerable.Range(minimumYear, DateTime.Now.Year - minimumYear + 1)
                .OrderByDescending(x => x)
                .Select(x => new SelectListItem
                {
                    Text = x.ToString(),
                    Value = x.ToString()
                })
                .ToList();

            if (!string.IsNullOrEmpty(allLabel))
            {
                years.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = allLabel
                });
            }

            return years;
        }

        public static List<SelectListItem> GetEnumSelectList<TEnum>(string? allLabel = null, bool sort = true)
            where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum))
                             .Cast<TEnum>();

            var items = values.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = GetDisplayName(x)
            });

            if (sort)
            {
                items = items.OrderBy(x => x.Text);
            }

            var list = items.ToList();

            if (!string.IsNullOrEmpty(allLabel))
            {
                list.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = allLabel
                });
            }

            return list;
        }

        public static async Task<List<SelectListItem>> GetDistinctEnumSelectListAsync<TEntity, TEnum>(
            IQueryable<TEntity> query, Expression<Func<TEntity, TEnum>> selector, string? allLabel = null,
            bool sort = true) where TEnum : Enum
        {
            var values = await query.Select(selector).Distinct().ToListAsync();

            var items = values.Select(x => new SelectListItem
            {
                Value = Convert.ToInt32(x).ToString(),
                Text = GetDisplayName(x)
            });

            if (sort)
            {
                items = items.OrderBy(x => x.Text);
            }

            var list = items.ToList();

            if (!string.IsNullOrEmpty(allLabel))
            {
                list.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = allLabel
                });
            }

            return list;
        }

        private static string GetDisplayName(Enum enumValue) 
        { 
            var member = enumValue.GetType().GetMember(enumValue.ToString()).First(); 
            var displayAttr = member.GetCustomAttribute<DisplayAttribute>(); 
            return displayAttr?.Name ?? enumValue.ToString(); 
        }
    }
}
