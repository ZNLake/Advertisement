//This library consists of Structs and Classes used to interact with the
//database and the front end in order to gather the required data to produce
//tailored ads to an existing user who has made purchases and random ads to a
//new/existing user






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

        public double prod_price;
        public string prod_name;
        public string prod_categ;
        public string url;
    }

    public struct retrieved_Data
    {
        //used to load historical purchase habit data
        public retrieved_Data()
        {
            avg_prod_price = double.MinValue;
            subtotal = int.MinValue;
            categ_freq = new Dictionary<string, int>();

        }
        public double avg_prod_price;
        public int subtotal;
        public Dictionary<string, int> categ_freq;

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

        protected UserPurchases()
        {
            //load historical data and product freqeuency from database
        }

        protected UserPurchases(int user_id)
        {
            this.user_id = user_id;
            session_avg_product_price = 0;
            session_subtotal = 0;

            retrieved_Data userHistory = new retrieved_Data();
            // retrieved_Data hist_data = isaac function 

            //use struct to store data histroical ata
            historical_avg_prod_price = 0;
            historical_subtotal = 0;
            category_frequency.Clear();
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
    }


    //class that uses UserPurchases data to predict and send correct ads to Cart and Product pages 
    public class AdPrediction : UserPurchases
    {
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


        AdPrediction()
        {


        }
        string requestAd()
        {

            //if existing user
            category_frequency = category_frequency.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            var count = 2;
            prodRequest new_prods = new prodRequest(category_frequency.Keys.Take(count).ToList(), historical_avg_prod_price); ;
            //call ad generator

            //else
            // request random ad generator
            // requestAd();
            // return provided ad to zeb 

            return "string";

        }

    }
}
