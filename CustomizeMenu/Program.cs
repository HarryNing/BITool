using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;
using System.Configuration;

namespace CustomizeMenu
{
    class Program
    {
        static void Main(string[] args)
        {
            string spSite = ConfigurationManager.AppSettings["site"].ToString();
            Console.WriteLine(spSite);
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(spSite))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        if (!web.Navigation.UseShared)
                        {
                            string resTitle = "Home";
                            string resUrl = web.Navigation.Home.Url;
                            string reportsTitle = "Reports";
                            string reportsUrl = "http://e1reports.ef.com";

                            // Get the top link bar.
                            SPNavigationNodeCollection topnav = web.Navigation.TopNavigationBar;

                            // If a Resources link exists, get it.
                            SPNavigationNode homeLink = topnav
                                .Cast<SPNavigationNode>()
                                .FirstOrDefault(n => n.Title == resTitle);

                            // If the Resources link does not exist, create it.
                            if (homeLink == null)
                            {
                                homeLink = new SPNavigationNode(resTitle, resUrl);
                                homeLink = topnav.AddAsLast(homeLink);
                            }
                            if (homeLink != null)
                            {
                                Console.WriteLine(homeLink.Title);
                            }
                            // If the Resources node has a SharePoint Dev Center child, get it.
                            SPNavigationNode reportsLink = homeLink
                                .Children
                                .Cast<SPNavigationNode>()
                                .FirstOrDefault(n => n.Title == reportsTitle);

                            // If the item does not exist, create it.
                            if (reportsLink == null)
                            {
                                reportsLink = new SPNavigationNode(reportsTitle, reportsUrl, true);
                                reportsLink = homeLink.Children.AddAsLast(reportsLink);
                            }
                            else
                            {
                                Console.WriteLine(reportsLink.Url);
                            }
                        }
                    }
                }
            });

            Console.Write("\nPress ENTER to continue....");
            Console.ReadLine();
        }
    }
}