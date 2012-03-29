using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace meituan.Model
{
    public class DataItem
    {
        
        public DataItem(string title)
        {
            Title = title;
        }
        public DataItem()
        {
        }
        public string Title
        {
            get;
            private set;
        }



        public List<City> getCityList()
        {
            List<City> list = new List<City>();

            XElement xml = XElement.Load("DataSource/divisions.xml");

            foreach (XElement element2 in xml.Element("divisions").Elements("division"))
            {
                list.Add(new City() { Name = element2.Element("name").Value, Py = element2.Element("id").Value });
            }
            return list;
        }

    }

    public class City
    {
        public string Name { get; set; }
        public string Py { get; set; }
    }

    public class Deal
    {
        public string Deal_Id { get; set; }
        public string Deal_title { get; set; }
        public string Deal_img { get; set; }
        public string City_Name { get; set; }
        public string Deal_Url { get; set; }
        public string Deal_Price { get; set; }
        public string Value { get; set; }
        public string sales_num { get; set; }
        public string shop_long {get;set;}
        public string shop_lat {get;set;}
    }



}
