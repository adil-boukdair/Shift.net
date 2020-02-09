using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BiomedicaLib.Net.RestFul
{
    public class ExternalWebrequestbuilder
{
    string rooturl;
    public ExternalWebrequestbuilder(string rooturl)
    {
        this.rooturl=rooturl;
    }

    public static string ConstructQueryString(System.Collections.Specialized.NameValueCollection Params)
    {
        List<string> items = new List<string>();
        foreach (string name in Params)
            items.Add(String.Concat(name, "=", HttpUtility.UrlEncode(Params[name])));
        return string.Join("&", items.ToArray());
    }
    public static string ConstructQueryString(Object data)
    {

        Type t = data.GetType();
        System.Collections.Specialized.NameValueCollection nvc=new System.Collections.Specialized.NameValueCollection();
        foreach (var p in t.GetProperties())
        {
            var name = p.Name;
            var value=p.GetValue(data,null).ToString();
            nvc.Add(name, value);
        }
        return ConstructQueryString(nvc);
    }
    
    public String Get(Object param)
    {
        String R="";
        var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(rooturl+"?"+ConstructQueryString(param));
        httpWebRequest.Method = "GET";
       
        var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
          {
            var result = streamReader.ReadToEnd();
             R = result.ToString();
           }
      
        return R;
    }
    public String Post(Object param)
    {
        String R="";
        var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(rooturl+"?"+ConstructQueryString(param));
        httpWebRequest.Method = "POST";
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
          {
            var result = streamReader.ReadToEnd();
             R = result.ToString();
           }
      
        return R;
    }
     public String Put(Object param)
    {
        String R="";
        var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(rooturl+"?"+ConstructQueryString(param));
        httpWebRequest.Method = "PUT";
       httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
          {
            var result = streamReader.ReadToEnd();
             R = result.ToString();
           }
      
        return R;
    }
     public String Delete(Object param)
    {
        String R="";
        var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(rooturl+"?"+ConstructQueryString(param));
        httpWebRequest.Method = "DELETE";
        var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
          {
            var result = streamReader.ReadToEnd();
             R = result.ToString();
           }
      
        return R;
    }
}
}
