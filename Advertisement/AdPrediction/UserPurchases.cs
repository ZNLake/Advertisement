//This library consists of Structs and Classes used to interact with the
//database and the front end in order to gather the required data to produce
//tailored ads to an existing user who has made purchases and random ads to a
//new/existing user

using Database;
using ImageTransformer;
using System.Drawing;

namespace Advertisement.AdPrediction
{
    //used to collect data about a users purchase
    public struct cart_Product
    {

        public cart_Product(double prod_price, string prod_name, string prod_categ, string url = "")
        {

            this.prod_price = prod_price;
            this.prod_name = prod_name;
            this.prod_categ = prod_categ;
            this.url = url;

        }

        public double prod_price { get; set; }
        public string prod_name { get; set; }
        public string prod_categ { get; set; }
        public string url { get; set; }

    }


    public struct retrieved_Data
    {
        //used to load historical purchase habit data
        public retrieved_Data()
        {
            avg_prod_price = double.MinValue;
            subtotal = double.MinValue;
            categ_freq = new Dictionary<string, int>();

        }
        public double avg_prod_price;
        public double subtotal;
        public Dictionary<string, int> categ_freq;

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


    //class that runs simple calculations and monitors category frequency
    public class UserPurchases
    {
        protected int user_id;

        protected Dictionary<string, int> category_frequency = new Dictionary<string, int>();

        protected double session_avg_product_price;
        protected double session_subtotal;
        protected double historical_avg_prod_price;
        protected double historical_subtotal;

     
        protected UserPurchases(int user_id, bool is_new_user)
        {
            this.user_id = user_id;
            session_avg_product_price = 0;
            session_subtotal = 0;

            //check if this is a new user to see if we have to retrieve
            if(!is_new_user)
            {
                retrieved_Data user_history = new retrieved_Data();
                user_history = DatabaseLib.GetUserData(user_id);
                historical_avg_prod_price = user_history.avg_prod_price;
                historical_subtotal = user_history.subtotal;
                category_frequency = user_history.categ_freq;
            }else
            {
                historical_avg_prod_price = double.MinValue;
                historical_subtotal = double.MinValue;
                DatabaseLib.CreateUser(user_id);
            }
          
        }

        public void AddFrequency(List<cart_Product> session_purchases)
        {

            int frequency = 1;
            foreach (var key in category_frequency.Keys)
            {
                foreach (var p in session_purchases)
                {
                    if (p.prod_categ == key)
                    {
                        category_frequency[key] += frequency;
                    }
                    else
                    {
                        category_frequency.Add(p.prod_categ, frequency);
                    }
                }
            }
        }

        public void calculateAverages(string subtotal, List<cart_Product> products)
        {
            int historical_count = 0;

            session_subtotal = Int32.Parse(subtotal);
            session_avg_product_price = session_subtotal / products.Count();

            foreach (var key in category_frequency.Keys)
            {
                historical_count += category_frequency[key];
            }

            historical_avg_prod_price = (session_subtotal + historical_subtotal) / (products.Count() + historical_count);
        }
        public Image requestAd(int width, int height)
        {
            category_frequency = category_frequency.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            var count = 2;
            List<cart_Product> predicted_ads = new List<cart_Product>();
            prodRequest new_prods = new prodRequest(category_frequency.Keys.Take(count).ToList(), historical_avg_prod_price); ;
            predicted_ads = DatabaseLib.GetClosestPricedProducts(new_prods);
            AdCreative.AdCreativeTransformer(width, height, predicted_ads);    
            return AdCreative.ReturnAd(predicted_ads[0].prod_name);

        }
    }
}



