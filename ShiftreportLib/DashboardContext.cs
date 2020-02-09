using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftreportLib
{
	public class DashboardContext 
		: DbContext
	{
		public DashboardContext() : base("name=shiftreport_dashboard")
		{
			Database.SetInitializer<DashboardContext>(null);
		}

	}
}
