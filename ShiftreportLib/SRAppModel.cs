
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.ComponentModel.DataAnnotations.Schema;


namespace shiftreports.dashboard
{

	public class DashboardAppModel : DbContext
	{

		public DashboardAppModel() : base("name=DefaultConnection")
		{
			Database.SetInitializer<DashboardAppModel>(null);
		}

		public virtual DbSet<shiftreportapp.data.AppModel.empolyee_app_details> empolyee_app_details2 { set; get; }
		public virtual DbSet<shiftreportapp.data.AppModel.webapps_mst> webapps_mst2 { set; get; }

	}

}


	
	
