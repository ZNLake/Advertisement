using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Numerics;
using Advertisement.Models;

namespace Advertisement
{
    public static class DatabaseLib
    {
        public static void Shell()
        {
            using var context = DataContext.Instance;

            context.SaveChanges();
        }
    }
}

//namespace Advertisement
//{
//    public class Database
//    {
//    }
//}