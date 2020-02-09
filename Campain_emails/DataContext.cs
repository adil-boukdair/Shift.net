
// Add the below line to the Db Context file
/*

In the AppConfig / WebConfig use the bellow
<connectionStrings>
		<add name="app_insight_mst" connectionString="" providerName=""/>
	</connectionStrings>


*/
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

public class email_campain_status_mst_Context : DbContext
{
	public email_campain_status_mst_Context()
		: base("name=shiftreports")
	{
	}

	public virtual DbSet<email_campain_status_mst> email_campain_status_mst { set; get; }
}

public class DB_email_campain_status_mst
{


	public virtual DbSet<email_campain_status_mst> email_campain_status_mst { set; get; }

	public void Addemail_campain_status_mst(email_campain_status_mst data)
	{
		email_campain_status_mst_Context Context = new email_campain_status_mst_Context();
		Context.email_campain_status_mst.Add(data);
		Context.SaveChanges();
		Context.Dispose();
	}

	public email_campain_status_mst GetWaitingQ()
	{
		email_campain_status_mst_Context Context = new email_campain_status_mst_Context();
		return (from x in Context.email_campain_status_mst
				where x.status=="P"
				select x).FirstOrDefault();
	}

	public email_campain_status_mst Getemail_campain_status_mst(int id)
	{
		email_campain_status_mst_Context Context = new email_campain_status_mst_Context();
		return (from x in Context.email_campain_status_mst
				where x.Id == id
				select x).FirstOrDefault();
	}

	public void Saveemail_campain_status_mst(int id, email_campain_status_mst data)
	{
		email_campain_status_mst_Context Context = new email_campain_status_mst_Context();
		Context.Entry(data).State = System.Data.Entity.EntityState.Modified;
		Context.SaveChanges();
		Context.Dispose();
	}

	public void Deleteemail_campain_status_mst(email_campain_status_mst data)
	{
		email_campain_status_mst_Context Context = new email_campain_status_mst_Context();
		Context.email_campain_status_mst.Remove(data);
		Context.SaveChanges();
		Context.Dispose();
	}
}

public class email_campain_status_mst
{
	[Key]
	public int Id { set; get; }
	public string email_address { set; get; }
	public string status { set; get; }
}




