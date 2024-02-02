using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SRGD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Positions : ViewComponent
    {
        private IConfiguration _config;
        private Tools _Tools;

        public Positions(IConfiguration config, Tools Tools)
        {
            _Tools = Tools;
            _config = config;
        }
        public Task<IViewComponentResult> InvokeAsync(string ST,string EN,string RS,string RAT)
        {
            string rs = _Tools.mask(RS);
            SqlConnection con = new SqlConnection(_config.GetConnectionString("MASTER"));
            DataTable dt = new DataTable();
            dt.Columns.Add("Start Position", typeof(Int64));
            dt.Columns.Add("End Position", typeof(Int64));
            SqlCommand command = new SqlCommand(String.Format("SELECT SPOSITION,EPOSITION FROM {0} WHERE STARTING= '"+ST+"' AND ENDING='"+EN+"' AND REPETITIVE_SEQUENCE='"+rs+"' ORDER BY SPOSITION", RAT), con);
            con.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = reader[0];
                    dr[1] = reader[1];
                    dt.Rows.Add(dr);
                }

            }
            reader.Close();
            con.Close();
            return Task.FromResult<IViewComponentResult>(View("Positions",dt));
        }

    }
}
