// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using AiCodo.Codes;
using AiCodo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCodo.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public SqlData SqlData { get; } = SqlData.Current;

        public SqlConnection ConnItem { get; set; }

        public TableSchema Table { get; set; }

        public List<CodeTemplateItem> TemplateItems { get;private set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet(string? table, string? conn)
        {
            var connItem = conn.IsNullOrEmpty() ? SqlData.Connections.FirstOrDefault() :
                SqlData.Connections[conn];
            if (connItem == null)
            {
                return Page();
            }
            ConnItem = connItem;

            var tableItem = table.IsNullOrEmpty() ? connItem.Tables.FirstOrDefault() :
                connItem.Tables[table];
            if(tableItem == null)
            {
                return NotFound();
            }
            Table = tableItem;
            ResetTemplateItems();
            return Page();
        }

        public IActionResult OnGetCode(string? table, string? conn,string? name)
        { 
            var connItem = conn.IsNullOrEmpty() ? SqlData.Connections.FirstOrDefault() :
                SqlData.Connections[conn];
            if (connItem == null)
            {
                return NotFound();
            }

            var tableItem = table.IsNullOrEmpty() ? connItem.Tables.FirstOrDefault() :
                connItem.Tables[table];
            if (tableItem == null)
            {
                return NotFound();
            }
            var template = CodeSetting.Current.Templates
                .FirstOrDefault(f=>f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if(template == null)
            {
                return NotFound();
            }
            var code = "";
            switch (template.ModelType)
            {
                case "Table":
                    code = CodeService.CreateCodeWithTemplate(tableItem, template.FileName);
                    break;
                case "SqlTable":
                    var sqlTable = SqlData.Current.GetSqlTable(table,conn);
                    code = CodeService.CreateCodeWithTemplate(sqlTable, template.FileName);
                    break;
                default:
                    break;
            }
            //var model =  tableItem;
            //var entity = CodeService.CreateCodeWithTemplate(model, template.FileName);
            Response.Headers.Add("Content-Disposition", $"attachment;filename={tableItem.CodeName}.cs"); 
            return new ContentResult { Content = code,  ContentType = "application /octet-stream" }; 
        }

        private void ResetTemplateItems()
        {
            TemplateItems = CodeSetting.Current.Templates.Where(t => t.ModelType.Equals("Table") ||t.ModelType.Equals("SqlTable")).ToList();
        }
    }
}
