using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;

namespace AIBot_FYP16004
{
    //Specify LUIS ID and Subscription Key to connect to LUIS generic cantonese model
    [LuisModel("51bd7915-e0b1-4ef4-b648-fb4471d66d96", "e4db71753e864294a1f49256a07647b9")]
    [Serializable]
    public class GenericQueryDialog : LuisDialog<DeviceOrder2>
    {
        private readonly BuildFormDelegate<DeviceOrder2> MakeDeviceForm;

        internal GenericQueryDialog(BuildFormDelegate<DeviceOrder2> makeDeviceForm)
        {
            this.MakeDeviceForm = makeDeviceForm;
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string message = "你好,你想要部點樣嘅電腦?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            //Randomly picked a reply from 3 pre-defined values
            Random random = new Random();
            int randomNumber = random.Next(0, 3);

            string message;
            switch (randomNumber)
            {
                case 0:
                    message = "唔好意思，我暫時未明你啱啱講嘅嘢。不如試下問問我有咩電腦好推介吖?";
                    break;
                case 1:
                    message = "我諗…可能我要搵一搵外援嚟應答你啱啱講嘅嘢。不如試下問問我有咩電腦好推介吖?";
                    break;
                case 2:
                    message = "喺介紹心水電腦比你嘅方面我會係專家，但係問到其他嘢就…我都係扮下傻瓜";
                    break;
                default:
                    message = "唔好意思，我唔明白";
                    break;
            }
            
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Satisfied")]
        public async Task Satisfied(IDialogContext context, LuisResult result)
        {
            string message = "多謝你同我對話. Byebye";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Unsatisfied")]
        public async Task Unsatisfied(IDialogContext context, LuisResult result)
        {
            /*
            string message = "User indicated unsatisfied";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            */
            var deviceForm = new FormDialog<DeviceOrder2>(new DeviceOrder2(), this.MakeDeviceForm, FormOptions.PromptInStart);
            context.Call<DeviceOrder2>(deviceForm, DeviceFormComplete);
        }


        private async Task DeviceFormComplete(IDialogContext context, IAwaitable<DeviceOrder2> result)
        {
            DeviceOrder2 order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("你取消左");
                return;
            }

            if (order != null)
            {
                //Added for displaying db query result
                string message = "";

                //Try to send the recommendation with a picture
                IMessageActivity replyMessage = context.MakeMessage();//Get the message activity in context

                replyMessage.Attachments = new List<Attachment>();//Attachments

                List<AIBot_FYP16004.Models.DeviceCatalog> productList = searchDB(order);//[0].Name

                int num_of_products = productList.Count;
                int products_to_show = num_of_products > 3 ? 3 : num_of_products;

                //Check if the number of products returned is 0 
                if (num_of_products == 0)
                {
                    message = "唔好意思，冇相關產品推薦吖";
                }
                else
                {
                    message = "我地嘅搜索結果喺";
                    try
                    {
                        for (int i = 0; i < products_to_show; i++)
                        {

                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: "https://c.s-microsoft.com/en-us/CMSImages/lrn-share-site-ms-logo.png?version=bf62922f-fda3-d328-e220-b699eac0d6c0"));

                            List<CardAction> cardButtons = new List<CardAction>();

                            CardAction plButton = new CardAction()
                            {
                                Value = "https://www.microsoftstore.com.hk/",
                                Type = "openUrl",
                                Title = "產品頁面"
                            };

                            cardButtons.Add(plButton);

                            HeroCard plCard = new HeroCard()
                            {
                                Title = productList[i].Name,
                                Subtitle = productList[i].DeviceDescription,
                                Images = cardImages,
                                Buttons = cardButtons
                            };

                            Attachment plAttachment = plCard.ToAttachment();

                            replyMessage.Attachments.Add(plAttachment);

                        }
                    }
                    catch (Exception ex)
                    {
                        message += ("Exception Message: " + ex.Message);
                        message += ("Exception StackTrace: " + ex.StackTrace);
                    }
                    //Ended
                }


                replyMessage.Text = message;

                await context.PostAsync(replyMessage);
                await context.PostAsync("多謝你對我嘅支持!");
                //await context.PostAsync("Your Device Order: " + order.ToString());
                //await context.PostAsync("Brand value"+order.Brand_value.ToString());
                //await context.PostAsync("Device type" + order.type_value.ToString());
                //await context.PostAsync("Search result: " + SearchDB(order));

            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Desktop")]
        [LuisIntent("Tablet")]
        [LuisIntent("Notebook")]
        [LuisIntent("2in1")]
        [LuisIntent("GamingDevices")]
        public async Task Categorization(IDialogContext context, LuisResult result)
        {
            //To get the index of the intent with highest score
            int intent_count = result.Intents.Count;
            int index = 0;
            double? score = 0;

            for (int i = 0; i < intent_count; i++)
            {
                if (result.Intents[i].Score > score)
                {
                    score = result.Intents[i].Score;
                    index = i;
                }
            }

            //Intent of highest score
            string category = result.Intents[index].Intent;
            //string message = "分類為: "+ category + " 我地建議：";
            string message = "";

            //Try to send the recommendation with a picture
            IMessageActivity replyMessage = context.MakeMessage();//Get the message activity in context

            replyMessage.Attachments = new List<Attachment>();//Attachments

            List<AIBot_FYP16004.Models.DeviceCatalog> productList = get_recommendation(category);//[0].Name
            int num_of_products = productList.Count;
            int products_to_show = num_of_products > 3 ? 3 : num_of_products;

            //Check if the number of products returned is 0 
            if (num_of_products == 0)
            {
                message = "唔好意思，冇相關產品推薦吖";
            }
            else
            {
                message = "以下喺我地嘅建議：";

                try
                {
                    for (int i = 0; i < products_to_show; i++)
                    {

                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "https://c.s-microsoft.com/en-us/CMSImages/lrn-share-site-ms-logo.png?version=bf62922f-fda3-d328-e220-b699eac0d6c0"));

                        List<CardAction> cardButtons = new List<CardAction>();

                        CardAction plButton = new CardAction()
                        {
                            Value = "https://www.microsoftstore.com.hk/",
                            Type = "openUrl",
                            Title = "產品頁面"
                        };

                        cardButtons.Add(plButton);

                        HeroCard plCard = new HeroCard()
                        {
                            Title = productList[i].Name,
                            Subtitle = productList[i].DeviceDescription,
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();

                        replyMessage.Attachments.Add(plAttachment);

                    }
                }
                catch (Exception ex)
                {
                    message += ("Exception Message: " + ex.Message);
                    message += ("Exception StackTrace: " + ex.StackTrace);
                }
                //End

                //Add a button to ask whether the user is satisfied with the recommendations or not
                List<CardAction> sButtons = new List<CardAction>();

                CardAction sButton1 = new CardAction()
                {
                    Value = "滿意",
                    Type = "imBack",
                    Title = "滿意"
                };

                CardAction sButton2 = new CardAction()
                {
                    Value = "唔滿意",
                    Type = "imBack",
                    Title = "唔滿意"
                };

                sButtons.Add(sButton1);
                sButtons.Add(sButton2);

                HeroCard sCard = new HeroCard()
                {
                    Title = "滿意我地嘅推薦?",
                    Buttons = sButtons
                };

                replyMessage.Attachments.Add(sCard.ToAttachment());
        }

            replyMessage.Text = message;

            await context.PostAsync(replyMessage);
            context.Wait(MessageReceived);
        }


        public const string Entity_Complaint_Product = "Product";
        public const string Entity_Complaint_Service = "Service";

        [LuisIntent("Complaint")]
        public async Task Complaint(IDialogContext context, LuisResult result)
        {
            string message="";
            string product = "";
            string service = "";

            EntityRecommendation Product;
            if (result.TryFindEntity(Entity_Complaint_Product, out Product))
            {
                product = Product.Entity;
                //message += "Complaint Product: " + Product.Entity;
            }

            EntityRecommendation Service;
            if (result.TryFindEntity(Entity_Complaint_Service, out Service))
            {
                service = Service.Entity;
                //message += "Complaint Service: " + Service.Entity;
            }

            //Handle the complaint
            handle_complaint(context.Activity, product, service, result.Query);
            message += "你嘅投訴已經收到，我地將會盡快處理";
            /*
            //message += "Conversation Channel id: " + context.Activity.Conversation.Id;
            //message += "Conversation Channel name: " + context.Activity.Conversation.Name;
            message += "User query: " + result.Query;
            message += "Channel name: " + context.Activity.ChannelId;
            //message += "Service URL: " + context.Activity.ServiceUrl;
            message += "User Channel id: " + context.Activity.From.Id;
            message += "User Name: " + context.Activity.From.Name;
            */

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        private List<AIBot_FYP16004.Models.DeviceCatalog> searchDB(DeviceOrder2 order)
        {
            Models.CustomerBotDBEntities1 DB = new Models.CustomerBotDBEntities1();
            var products = (from DeviceCatalog in DB.DeviceCatalogs select DeviceCatalog).ToList();
            var brand = order.Brand_value.ToString();
            var category = order.type_value.ToString();
            var price_range = order.price_range.ToString();
            if (brand!="Unknown")
                products = (from product in products where product.Manufacturer.Contains(brand) select product).ToList();
            if (category != "Unknown")
            {
                if(category=="twoInOne")
                    products = (from product in products where product.FormFactor.Contains("2-in-1") select product).ToList();
                else if(category=="Desktop")
                    products = (from product in products where product.FormFactor.Contains("Desktop") select product).ToList();
                else if(category=="GamingDevices")
                    products = (from product in products where product.FormFactor.Contains("Gaming Device") select product).ToList();
                else if(category=="Notebook")
                    products = (from product in products where product.FormFactor.Contains("Notebook") select product).ToList();
                else if(category=="Tablet")
                    products = (from product in products where product.FormFactor.Contains("2-in-1 Detachable") select product).ToList();
            }
            if(price_range=="Range1")
                products = (from product in products where product.PriceUSD*7.8>=5000 && product.PriceUSD*7.8<=8000 select product).ToList();
            else if (price_range=="Range2")
                products = (from product in products where product.PriceUSD*7.8 >=8000 && product.PriceUSD*7.8<=12000 select product).ToList();
            else if (price_range=="Range3")
                products = (from product in products where product.PriceUSD*7.8>=12000 && product.PriceUSD*7.8<=15000 select product).ToList();
            else if (price_range=="Range4")
                products = (from product in products where product.PriceUSD*7.8>=15000 select product).ToList();
            return products;
        }

        private List<AIBot_FYP16004.Models.DeviceCatalog> get_recommendation(string category)
        {
            Models.CustomerBotDBEntities1 DB = new Models.CustomerBotDBEntities1();
            var products = (from DeviceCatalog in DB.DeviceCatalogs select DeviceCatalog).ToList();
            //List<AIBot_FYP16004.Models.DeviceCatalog> results = null;
            var results = products;
            Random rnd = new Random();
            if (category == "2in1")
                products = (from product in products where product.FormFactor.Contains("2-in-1") select product).ToList();
            else if (category == "Desktop")
                products = (from product in products where product.FormFactor.Contains("Desktop") select product).ToList();
            else if (category == "Notebook")
                products = (from product in products where product.FormFactor.Contains("Notebook") select product).ToList();
            else if (category == "Tablet")
                products = (from product in products where product.FormFactor.Contains("2-in-1 Detachable") select product).ToList();
            else if (category == "GamingDevices")
            {
                results = (from product in products select product).OrderByDescending(x => x.PriceUSD).Take(4).ToList();
                products = (from product in products where product.FormFactor.Contains("Gaming Device") select product).ToList();
                products.AddRange(results);
                return products;
            }
            var brands = products.Select(p => p.Manufacturer).Distinct().ToList();
            for(int i=0; i<5; i++)
            {
                var candidates = (from product in products where product.Manufacturer.Contains(brands[i]) select product).ToList();
                int index = rnd.Next(0, candidates.Count);
                if (i == 0)
                {
                    if(index>0)
                        results = candidates.Skip(index).Take(1).ToList();
                    else
                        results = candidates.Take(1).ToList();
                }
                else
                {
                    if (index > 0)
                        results.AddRange(candidates.Skip(index).Take(1));
                    else
                        results.AddRange(candidates.Take(1));
                }
            }
            return results;
        }

        private void handle_complaint(IActivity activity, string product, string service, string query)
        {
            Models.CustomerBotDBEntities1 DB = new Models.CustomerBotDBEntities1();
            Models.Complaint newComplaint = new Models.Complaint();
            newComplaint.Channel = activity.ChannelId;
            newComplaint.UserID = activity.From.Id;
            newComplaint.UserName = activity.From.Name;
            newComplaint.Time = DateTime.Now;
            newComplaint.Content = query;
            string type = "General";
            if (product != "")
                type = "Product";
            else if (service != "")
                type = "Service";
            newComplaint.Type = type;
            DB.Complaints.Add(newComplaint);
            DB.SaveChanges();
        }

    }
}