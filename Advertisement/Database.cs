using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Numerics;
using Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;

namespace Database;

public struct cart_Product
{

    public cart_Product(double prod_price, string prod_name, string prod_categ, string url = "")
    {

        this.prod_price = prod_price;
        this.prod_name = prod_name;
        this.prod_categ = prod_categ;
        this.url = url;

    }

    public double prod_price;
    public string prod_name;
    public string prod_categ;
    public string url;
}

public struct prodRequest
{
    public prodRequest(List<string> keys, double avg_price)
    {
        this.category = keys;
        this.avg_price = avg_price;
    }
    public List<string> category;
    public double avg_price;

}

public struct retrieved_Data
{
    public retrieved_Data()
    {
        avg_prod_price = double.MinValue;
        subtotal = int.MinValue;
        categ_freq = new Dictionary<string, int>();

    }
    public double avg_prod_price;
    public float subtotal;
    public Dictionary<string, int> categ_freq;
}


public static class DatabaseLib
{
    // function that takes an array and fills it with each row in monthly stats
    // [0, 1, 2, 3, 4, 5]
    // [month1clicks, month1conversions, month2clicks, month2conversions, month3clicks, month3conversions]

    public static string CategoryNumberToString(int categoryId)
    {
        using var context = DataContext.Instance;

        var categoryName = context.Categories
        .Where(c => c.Id == categoryId)
        .Select(c => c.Name)
        .FirstOrDefault();

        return categoryName;
    }

    public static int[] GetMonthlyStats()
    {
        using var context = DataContext.Instance;

        var statsList = context.MonthlyStats.ToList();

        int[] resultArray = new int[statsList.Count * 2];

        for (int i = 0; i < statsList.Count; i++)
        {
            resultArray[i * 2] = statsList[i].Clicks;
            resultArray[i * 2 + 1] = statsList[i].Conversions;
        }

        return resultArray;
    }

    public static Product GetProductById(int productId)
    {
        using var context = DataContext.Instance;

        var product = context.Products.FirstOrDefault(p => p.Pid == productId);

        return product;
    }

    public static string GetProductImageById(int productId)
    {
        using var context = DataContext.Instance;

        var product = context.Products.FirstOrDefault(p => p.Pid == productId);
        if (product != null)
        {
            return product.Image;
        }
        else
        {
            return null;
        }
    }

    public static string GetProductImageByName(string productName)
    {
        using var context = DataContext.Instance;

        var product = context.Products.FirstOrDefault(p => string.Equals(p.Name, productName));
        if (product != null)
        {
            return product.Image;
        }
        else
        {
            return null;
        }
    }

    public static Category GetCategoryById(int id)
    {
        using var context = DataContext.Instance;

        var category = context.Categories.FirstOrDefault(c => c.Id == id);

        return category;
    }

    public static Category GetCategoryByName(string categoryName)
    {
        using var context = DataContext.Instance;

        var category = context.Categories.FirstOrDefault(c => c.Name == categoryName);

        return category;
    }

    // finds 2 closest priced items within category and return the product struct
    // Needs the products and prodRequest classes to fix errors
    public static List<cart_Product> GetClosestPricedProducts(prodRequest request)
    {
        using var context = DataContext.Instance;

        var closestPricedProducts = new List<cart_Product>();

        var productsInCategory = context.Products
            .Where(p => request.category.Contains(CategoryNumberToString(p.Category)))
            .ToList();

        productsInCategory.Sort((p1, p2) =>
            Math.Abs(p1.Price - request.avg_price).CompareTo(Math.Abs(p2.Price - request.avg_price))
        );

        closestPricedProducts.Add(new cart_Product(productsInCategory[0].Price, productsInCategory[0].Name, CategoryNumberToString(productsInCategory[0].Category), productsInCategory[0].Image));
        closestPricedProducts.Add(new cart_Product(productsInCategory[1].Price, productsInCategory[1].Name, CategoryNumberToString(productsInCategory[1].Category), productsInCategory[1].Image));

        return closestPricedProducts;
    }

    public static User GetUserById(int userId)
    {
        using var context = DataContext.Instance;

        var user = context.Users.FirstOrDefault(u => u.Id == userId);

        return user;
    }

    public static retrieved_Data GetUserData(int userId)
    {
        using var context = DataContext.Instance;

        var userData = new retrieved_Data();

        var user = context.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            userData.subtotal = user.Subtotal;
        }

        var userPurchases = context.UserPurchases.Where(up => up.UserId == userId).ToList();
        foreach (var purchase in userPurchases)
        {
            var category = CategoryNumberToString(purchase.CategoryShopped);
            if (!userData.categ_freq.ContainsKey(category))
            {
                userData.categ_freq[category] = 1;
            }
            else
            {
                userData.categ_freq[category]++;
            }
        }

        var averageProductPrice = context.UserPurchases
            .Where(up => up.UserId == userId)
            .Average(up => up.AvgProductPrice);

        userData.avg_prod_price = averageProductPrice;

        return userData;
    }

    public static void IncrementClicks()
    {
        using var context = DataContext.Instance;

        var monthlyStats = context.MonthlyStats.FirstOrDefault();

        if (monthlyStats != null)
        {
            monthlyStats.Clicks += 1;
            context.SaveChanges();
        }
    }
}



