/*Francis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;

namespace AIBot_FYP16004
{

    //Specify LUIS ID and Subscription Key to connect to LUIS
    [LuisModel("4ae199ad-dac0-455c-a95b-3a0192e8e652", "e4db71753e864294a1f49256a07647b9")]
    [Serializable]
    public class ProductQueryDialog : LuisDialog<DeviceOrder>
    {
        private readonly BuildFormDelegate<DeviceOrder> MakeDeviceForm;

        internal ProductQueryDialog(BuildFormDelegate<DeviceOrder> makeDeviceForm)
        {
            this.MakeDeviceForm = makeDeviceForm;
        }

        //const strings of feature entities
        public const string Entity_Feature_Price = "Feature::PriceValue";
        public const string Entity_Feature_Brand = "Feature::Brand";
        public const string Entity_Feature_Processor = "Feature::Processor";
        public const string Entity_Feature_GC = "Feature::Graphic Card";
        public const string Entity_Feature_RAM = "Feature::RAM";
        public const string Entity_Feature_HD = "Feature::Hard Drive";

        //const strings of usage entities
        public const string Entity_Usage_PlayGames = "Usage::Play Games";
        public const string Entity_Usage_Video = "Usage::Video Editing";
        public const string Entity_Usage_Documentation = "Usage::Documentation Editing";

        //const strings of product entities
        public const string Entity_Product_Spectre = "Product::Spectre";
        public const string Entity_Product_SurfaceBook = "Product::Surface Book";
        public const string Entity_Product_SurfacePro = "Product::Surface Pro";
        public const string Entity_Product_Yoga = "Product::Yoga";
        public const string Entity_Product_Zenbook = "Product::Zenbook";

        //const strings of relationship entities
        public const string Entity_Relationship_Greater = "Relationship::Greater";
        public const string Entity_Relationship_Smaller = "Relationship::Smaller";
        //public const string Entity_Relationship_Equal = "Relationship::Equal";
        public const string Entity_Relationship_Approx = "Relationship::Approx";
        //public const string Entity_Relationship_With = "Relationship::With";
        public const string Entity_Relationship_Without = "Relationship::Without";

        //const string for composite
        public const string Entity_Composite = "CompositeFeature";

        //When the intent is classified as "None"
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = "Sorry I am not intelligent enough to understand your request. Could you please make it more understandable?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        //When the intent is classified as "Bye"
        [LuisIntent("Bye")]
        public async Task Bye(IDialogContext context, LuisResult result)
        {
            string message = "Have a nice day! Byebye.";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        //When the intent is classified as "Greeting"
        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string message = "Hi, what can I do for you?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        //When the intent is classified as "Complaint"
        [LuisIntent("Complaint")]
        public async Task Complaint(IDialogContext context, LuisResult result)
        {
            string message = "Please write down your detailed complaint in your following input. I will forward it to our manager";
            await context.PostAsync(message);
            context.Wait(HandleComplaint);
        }


        //Check whether the feature entity is in a composite feature. If yes, return the corresponding relationship as a string
        public string checkComposite(EntityRecommendation entity, LuisResult result)
        {
            EntityRecommendation composite;
            string relation=null;

            if (!result.TryFindEntity(Entity_Composite, out composite))
            {
                return null;
            }

            foreach (var compo in result.Entities)
            {
                if (compo.Type == Entity_Composite)
                {
                    if ((compo.EndIndex >= entity.EndIndex) && (compo.StartIndex <= entity.StartIndex))
                    {
                        relation = findRelation(compo.StartIndex, compo.EndIndex, result);
                        break;
                    }
                }
            }
            return relation;
        }

        //Find the relationship entity within the range of index start and end, return the relation as a string
        public string findRelation(int? start, int? end, LuisResult result)
        {
            //EntityRecommendation relation;
            foreach (var relation in result.Entities)
            {
                if (relation.Type==Entity_Relationship_Greater)
                {
                    if ((relation.StartIndex >= start) && (relation.EndIndex <= end))
                    {
                        return "Greater";
                    }
                }

                if (relation.Type == Entity_Relationship_Smaller)
                {
                    if ((relation.StartIndex >= start) && (relation.EndIndex <= end))
                    {
                        return "Smaller";
                    }
                }
                Francis*/
                /*
                if (result.TryFindEntity(Entity_Relationship_Equal, out relation))
                {
                    if ((relation.StartIndex >= start) && (relation.EndIndex <= end))
                    {
                        return "Equal";
                    }
                }
                */

                /*Francis
                if (relation.Type == Entity_Relationship_Approx)
                {
                    if ((relation.StartIndex >= start) && (relation.EndIndex <= end))
                    {
                        return "Approx";
                    }
                }

                if (relation.Type == Entity_Relationship_Without)
                {
                    if ((relation.StartIndex >= start) && (relation.EndIndex <= end))
                    {
                        return "Without";
                    }
                }
            }
            return null;
        }

        //When the intent is classified as "Recommendation" or "Purchase"
        [LuisIntent("Recommendation")]
        [LuisIntent("Purchase")]
        public async Task ProcessDeviceForm(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>();

            //Extract price in query to send to DeviceForm
            EntityRecommendation price;
            if (result.TryFindEntity(Entity_Feature_Price, out price))
            {
                //If preference on price exists in query, set "hasPrice" to "yes"
                entities.Add(new EntityRecommendation(type: "hasPrice") { Entity = "yes" });
                entities.Add(new EntityRecommendation(type: "price_value") { Entity = price.Entity });
                //Check whether there is a relation
                string relation = checkComposite(price,result);
                if (relation != null)
                {
                    entities.Add(new EntityRecommendation(type: "price_relation") { Entity = relation });
                }else
                {
                    entities.Add(new EntityRecommendation(type: "price_relation") { Entity = "Equal" });
                }
            }else
            {
                //If preference on price doesn't exist in query, set "hasPrice" to "no"
                entities.Add(new EntityRecommendation(type: "hasPrice") { Entity = "no" });
            }

            //Extract Processor in query to send to DeviceForm
            EntityRecommendation Processor;
            if (result.TryFindEntity(Entity_Feature_Processor, out Processor))
            {
                entities.Add(new EntityRecommendation(type: "hasProcessor") { Entity = "yes" });
                entities.Add(new EntityRecommendation(type: "Processor_value") { Entity = Processor.Entity });
            }

            //Extract Brand in query to send to DeviceForm
            EntityRecommendation Brand;
            if (result.TryFindEntity(Entity_Feature_Brand, out Brand))
            {
                entities.Add(new EntityRecommendation(type: "hasBrand") { Entity = "yes" });
                entities.Add(new EntityRecommendation(type: "Brand_value") { Entity = Brand.Entity });
            }

            //Extract RAM in query to send to DeviceForm
            EntityRecommendation RAM;
            if (result.TryFindEntity(Entity_Feature_RAM, out RAM))
            {
                entities.Add(new EntityRecommendation(type: "hasRAM") { Entity = "yes" });
                entities.Add(new EntityRecommendation(type: "RAM_value") { Entity = RAM.Entity });
                string relation = checkComposite(RAM, result);
                if (relation != null)
                {
                    entities.Add(new EntityRecommendation(type: "RAM_relation") { Entity = relation });
                }
                else
                {
                    entities.Add(new EntityRecommendation(type: "RAM_relation") { Entity = "Equal" });
                }
            }else
            {
                //If preference on RAM size doesn't exist in query, set "hasRAM" to "no"
                entities.Add(new EntityRecommendation(type: "hasRAM") { Entity = "no" });
            }

            //Extract RAM in query to send to DeviceForm
            EntityRecommendation HD;
            if (result.TryFindEntity(Entity_Feature_HD, out HD))
            {
                entities.Add(new EntityRecommendation(type: "hasHD") { Entity = "yes" });
                entities.Add(new EntityRecommendation(type: "HD_value") { Entity = HD.Entity });
                string relation = checkComposite(HD, result);
                if (relation != null)
                {
                    entities.Add(new EntityRecommendation(type: "HD_relation") { Entity = relation });
                }
                else
                {
                    entities.Add(new EntityRecommendation(type: "HD_relation") { Entity = "Equal" });
                }
            }else
            {
                //If preference on Hard Disk Space doesn't exist in query, set "hasHD" to "no"
                entities.Add(new EntityRecommendation(type: "hasHD") { Entity = "no" });
            }

            //Extract Graphic Card in query to send to DeviceForm
            EntityRecommendation GC;
            if (result.TryFindEntity(Entity_Feature_GC, out GC))
            {
                entities.Add(new EntityRecommendation(type: "hasGC") { Entity = "yes" });
            }

            //Extract usage in query to send to DeviceForm
            string usage=null;
            EntityRecommendation Luis_Usage;
            if (result.TryFindEntity(Entity_Usage_PlayGames, out Luis_Usage))
            {
                usage = "PlayGames";
            }else if (result.TryFindEntity(Entity_Usage_Video, out Luis_Usage))
            {
                usage = "VideoEditing";
            }
            else if (result.TryFindEntity(Entity_Usage_Documentation, out Luis_Usage))
            {
                usage = "DocumentationEditing";
            }
            else
            {
                usage = null;
            }

            if(usage!=null)
                entities.Add(new EntityRecommendation(type: "usage") { Entity = usage });

            //Extract model in query to send to DeviceForm
            string model = null;
            EntityRecommendation Luis_Model;
            if (result.TryFindEntity(Entity_Product_Spectre, out Luis_Model))
            {
                model = "Spectre";
            }
            else if (result.TryFindEntity(Entity_Product_SurfaceBook, out Luis_Model))
            {
                model = "SurfaceBook";
            }
            else if (result.TryFindEntity(Entity_Product_SurfacePro, out Luis_Model))
            {
                model = "SurfacePro";
            }
            else if (result.TryFindEntity(Entity_Product_Yoga, out Luis_Model))
            {
                model = "Yoga";
            }
            else if (result.TryFindEntity(Entity_Product_Zenbook, out Luis_Model))
            {
                model = "Zenbook";
            }
            else
            {
                model = null;
            }

            if(model!=null)
                entities.Add(new EntityRecommendation(type: "model") { Entity = model });

            //Go to the guided form dialog
            var deviceForm = new FormDialog<DeviceOrder>(new DeviceOrder(), this.MakeDeviceForm, FormOptions.PromptInStart, entities);
            context.Call<DeviceOrder>(deviceForm, DeviceFormComplete);

        }

        private async Task DeviceFormComplete(IDialogContext context, IAwaitable<DeviceOrder> result)
        {
            DeviceOrder order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the order!");
                return;
            }

            if (order != null)
            {
                await context.PostAsync("Your Device Order: " + order.ToString());
                await context.PostAsync("Searching for the product at present");
                await context.PostAsync("Search result: "+SearchDB(order));

            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            context.Wait(MessageReceived);
        }

        //Task to deal with complaint
        private async Task HandleComplaint(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var complaint = await argument;
            string result = StoreComplaint(complaint.Text);

            await context.PostAsync(result);
            await context.PostAsync("Is there any other thing I can do for you?");
            context.Wait(MessageReceived);
        }

        public class Product
        {
            public int Id { get; set; }
            public string Item_Name { get; set; }
            public string Processor { get; set; }
            public int RAM { get; set; }
            public bool Graphic_Card { get; set; }
            public int Hard_Drive { get; set; }
            public int Price { get; set; }
            public string Brand { get; set; }
        }

        //Search in db to find the products that meet the requirements in parameter order
        private string SearchDB(DeviceOrder order)
        {   Francis*/
            /*Models.BotDataEntities DB = new Models.BotDataEntities();
            var products = (from Catalogue in DB.Catalogues select Catalogue).ToList();
            //List<Product> products = new List<Product>
            //    {
            //       new Product {Id=1, Item_Name="Surface Pro 4 Intel Core m3 128GB 4GB RAM", Processor="m3", RAM=4, Graphic_Card=false, Hard_Drive=128, Price=6988, Brand="Microsoft"},
            //       new Product {Id=2, Item_Name="Surface Pro 4 Intel Core i5 128GB 4GB RAM", Processor="i5", RAM=4, Graphic_Card=false, Hard_Drive=128, Price=7888, Brand="Microsoft"},
            //       new Product {Id=3, Item_Name="Surface Pro 4 Intel Core i5 256GB 8GB RAM", Processor="i5", RAM=8, Graphic_Card=false, Hard_Drive=256, Price=10288, Brand="Microsoft"},
            //       new Product {Id=4, Item_Name="Surface Pro 4 Intel Core i7 256GB 8GB RAM", Processor="i7", RAM=8, Graphic_Card=false, Hard_Drive=256, Price=12388, Brand="Microsoft"},
            //       new Product {Id=5, Item_Name="Surface Pro 4 Intel Core i7 256GB 16GB RAM", Processor="i7", RAM=16, Graphic_Card=false, Hard_Drive=256, Price=13988, Brand="Microsoft"},
            //       new Product {Id=6, Item_Name="Surface Pro 4 Intel Core i7 512GB 16GB RAM", Processor="i7", RAM=16, Graphic_Card=false, Hard_Drive=512, Price=17088, Brand="Microsoft"},
            //       new Product {Id=7, Item_Name="Surface Pro 4 Intel Core i7 1TB 16GB RAM", Processor="i7", RAM=16, Graphic_Card=true, Hard_Drive=1024, Price=20888, Brand="Microsoft"},
            //       new Product {Id=8, Item_Name="Surface Book Intel Core i5 256GB 8GB RAM", Processor="i5", RAM=8, Graphic_Card=true, Hard_Drive=256, Price=14688, Brand="Microsoft"},
            //       new Product {Id=9, Item_Name="Surface Book Intel Core i7 256GB 8GB RAM", Processor="i7", RAM=8, Graphic_Card=true, Hard_Drive=256, Price=16288, Brand="Microsoft"},
            //       new Product {Id=10, Item_Name="Surface Book Intel Core i7 512GB 16GB RAM", Processor="i7", RAM=16, Graphic_Card=true, Hard_Drive=512, Price=20888, Brand="Microsoft"},
            //       new Product {Id=11, Item_Name="Surface Book Intel Core i7 1TB 16GB RAM", Processor="i7", RAM=16, Graphic_Card=true, Hard_Drive=1024, Price=24788, Brand="Microsoft"},
            //       new Product {Id=12, Item_Name="Lenovo Yoga 900-13ISK i5-6200U", Processor="i5", RAM=8, Graphic_Card=false, Hard_Drive=256, Price=11998, Brand="Lenovo"},
            //       new Product {Id=13, Item_Name="ASUS ZenBook UX305CA", Processor="m5", RAM=8, Graphic_Card=false, Hard_Drive=256, Price=9498, Brand="ASUS"},
            //       new Product {Id=14, Item_Name="HP Spectre x360 i7-6500U 512GB", Processor="i7", RAM=8, Graphic_Card=false, Hard_Drive=512, Price=13999, Brand="HP"}
            //    };
            //filter brand
            if (order.hasBrand.ToString() == "yes")
                products = (from product in products where product.Brand.Contains(order.Brand_value.ToString()) select product).ToList();
            //filter price value
            if (order.hasPrice.ToString()=="yes")
            {
                var price = Int32.Parse(Regex.Match(order.price_value, @"\d+").Value);
                if (order.price_relation.ToString()=="Greater")
                    products = (from product in products where product.Price>price select product).ToList();
                else if (order.price_relation.ToString()=="Smaller")
                    products = (from product in products where product.Price<price select product).ToList();
                else if (order.price_relation.ToString()=="Equal")
                    products = (from product in products where product.Price==price select product).ToList();
            }
            //filter price range
            if (order.needPrice.ToString()=="yes")
            {
                if (order.price_range.ToString()=="Range1")
                    products = (from product in products where product.Price>=2000 && product.Price<=5000 select product).ToList();
                else if (order.price_range.ToString() == "Range2")
                    products = (from product in products where product.Price>=5000 && product.Price<=8000 select product).ToList();
                else if (order.price_range.ToString() == "Range3")
                    products = (from product in products where product.Price>=8000 && product.Price<=10000 select product).ToList();
                else if (order.price_range.ToString() == "Range4")
                    products = (from product in products where product.Price>=10000 select product).ToList();
            }
            //filter processor
            if (order.hasProcessor.ToString()=="yes")
                products = (from product in products where product.Processor.Contains(order.Processor_value.ToString()) select product).ToList();
            //filter RAM value
            if (order.hasRAM.ToString() == "yes")
            {
                var RAM = Int32.Parse(Regex.Match(order.RAM_value, @"\d+").Value);
                if (order.RAM_relation.ToString()=="Greater")
                    products = (from product in products where product.Price>RAM select product).ToList();
                else if (order.RAM_relation.ToString()=="Smaller")
                    products = (from product in products where product.Price<RAM select product).ToList();
                else if (order.RAM_relation.ToString()=="Equal")
                    products = (from product in products where product.Price==RAM select product).ToList();
            }
            //filter RAM range
            if (order.needRAM.ToString()=="yes")
            {
                if (order.RAM_range.ToString()=="Range1")
                    products = (from product in products where product.RAM<4 select product).ToList();
                else if (order.RAM_range.ToString()=="Range2")
                    products = (from product in products where product.RAM>=4 && product.RAM<=8 select product).ToList();
                else if (order.RAM_range.ToString()=="Range3")
                    products = (from product in products where product.RAM>=8 && product.RAM<=16 select product).ToList();
                else if (order.RAM_range.ToString()=="Range4")
                    products = (from product in products where product.RAM>=16 select product).ToList();
            }
            //filter HD value
            if (order.hasHD.ToString()=="yes")
            {
                var HD = Int32.Parse(Regex.Match(order.HD_value, @"\d+").Value);
                if (order.HD_relation.ToString()=="Greater")
                    products = (from product in products where product.Hard_Drive>HD select product).ToList();
                else if (order.RAM_relation.ToString() == "Smaller")
                    products = (from product in products where product.Hard_Drive<HD select product).ToList();
                else if (order.RAM_relation.ToString() == "Equal")
                    products = (from product in products where product.Hard_Drive==HD select product).ToList();
            }
            //filter HD range
            if (order.needHD.ToString()=="yes")
            {
                if (order.HD_range.ToString()=="Range1")
                    products = (from product in products where product.Hard_Drive<250 select product).ToList();
                else if (order.HD_range.ToString()=="Range2")
                    products = (from product in products where product.Hard_Drive>=250 && product.Hard_Drive<=500 select product).ToList();
                else if (order.HD_range.ToString()=="Range3")
                    products = (from product in products where product.Hard_Drive>=500 && product.Hard_Drive<=800 select product).ToList();
                else if (order.HD_range.ToString()=="Range4")
                    products = (from product in products where product.Hard_Drive>800 select product).ToList();
            }
            //filter Graphics Card
            if (order.hasGC.ToString()=="yes")
                products = (from product in products where product.Graphic_Card==true select product).ToList();
            //filter usage
            if (order.usage.ToString()=="PlayGames")
                products = (from product in products where product.RAM>16 && product.Processor.Contains("i7") && product.Graphic_Card==true select product).ToList();
            else if (order.usage.ToString()=="VideoEditing")
                products = (from product in products where product.RAM>16 && product.Processor.Contains("i7") && product.Graphic_Card==true && product.Hard_Drive>512 select product).ToList();
            else if (order.usage.ToString()=="DocumentationEditing")
                products = (from product in products where product.RAM<=8 && !(product.Processor.Contains("i7")) && product.Price<10000 select product).ToList();
            //filter model
            if (order.model.ToString()== "Spectre")
                products = (from product in products where product.Item_Name.Contains("Spectre") select product).ToList();
            else if (order.model.ToString()=="SurfaceBook")
                products = (from product in products where product.Item_Name.Contains("Surface Book") select product).ToList();
            else if (order.model.ToString()=="SurfacePro")
                products = (from product in products where product.Item_Name.Contains("Surface Pro") select product).ToList();
            else if (order.model.ToString()=="Yoga")
                products = (from product in products where product.Item_Name.Contains("Yoga") select product).ToList();
            else if (order.model.ToString()=="Zenbook")
                products = (from product in products where product.Item_Name.Contains("ZenBook") select product).ToList();
            //output
            if (products.Count() == 0)
                return "No matched product lol.";
            else
                return products[0].Item_Name;*/
            /*Francis
            return "To be developed";
        }

        //Insert the complaint from users into db
        private string StoreComplaint(string complaint)
        {
            return "Complaint to store: "+complaint;
        }
        
    }
}
Francis*/