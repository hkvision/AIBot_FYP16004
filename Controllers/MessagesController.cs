using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Net.Http.Headers;

using Microsoft.Bot.Builder.FormFlow;
using Newtonsoft.Json.Linq;

namespace AIBot_FYP16004
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        //IForm class to ask users for unprovided information
        private static IForm<DeviceOrder2> BuildForm()
        {
            var builder = new FormBuilder<DeviceOrder2>();

            /*
            //Check whether has(feature) variables defined in Device.cs are "yes"
            ActiveDelegate<DeviceOrder> hasBrand = (device) => device.hasBrand == PreferenceOptions.yes;

            ActiveDelegate<DeviceOrder> hasPrice = (device) => device.hasPrice== PreferenceOptions.yes;
            ActiveDelegate<DeviceOrder> noPrice = (device) => device.hasPrice == PreferenceOptions.no;
            ActiveDelegate<DeviceOrder> needPrice = (device) => device.needPrice == PreferenceOptions.yes;

            ActiveDelegate<DeviceOrder> hasRAM = (device) => device.hasRAM == PreferenceOptions.yes;
            ActiveDelegate<DeviceOrder> noRAM = (device) => device.hasRAM == PreferenceOptions.no;
            ActiveDelegate<DeviceOrder> needRAM = (device) => device.needRAM == PreferenceOptions.yes;

            ActiveDelegate<DeviceOrder> hasHD = (device) => device.hasHD == PreferenceOptions.yes;
            ActiveDelegate<DeviceOrder> noHD = (device) => device.hasHD == PreferenceOptions.no;
            ActiveDelegate<DeviceOrder> needHD = (device) => device.needHD == PreferenceOptions.yes;

            ActiveDelegate<DeviceOrder> hasProcessor = (device) => device.hasProcessor == PreferenceOptions.yes;
            */

            //MessageDelegate<DeviceOrder> confirm_message = (device) => Task< new PromptAttribute(device.ToString())>;

            return builder
                //.Field("hasBrand")
                .Field("Brand_value")

                //.Field(nameof(DeviceOrder.model))

                //.Field("hasPrice")
                //.Field("price_relation", hasPrice)
                //.Field("price_value", hasPrice)

                //.Field("needPrice",noPrice)
                .Field("price_range")
                .Field("type_value")
                //.Field("hasProcessor")
                //.Field("Processor_value", hasProcessor)

                //.Field("hasRAM")
                //.Field("RAM_relation", hasRAM)
                //.Field("RAM_value", hasRAM)

                //.Field("needRAM", noRAM)
                //.Field("RAM_range", needRAM)

                //.Field("hasHD")
                //.Field("HD_relation", hasHD)
                //.Field("HD_value", hasHD)

                //.Field("needHD", noHD)
                //.Field("HD_range", needHD)

                //.Field("hasGC")

                //.Field(nameof(DeviceOrder.usage))
                //.AddRemainingFields()
                .Confirm("你滿意啱啱輸入嘅要求? 滿意，輸入'yes'，要修改，輸入'no'")
                //.Confirm("Would you like a\r\n Brand: {Brand_value},\r\n Model: {model},\r\n Price: {price_relation} {price_value},\r\n Processor: {Processor_value},\r\n RAM: {RAM_relation} {RAM_value},\r\n RAM Size Range: {RAM_range},\r\n Hard Disk: {HD_relation} {HD_value},\r\n Graphic Card: {hasGC},\r\n Usage: {usage} device?", hasPrice)
                //.Confirm("Would you like a\r\n Brand: {Brand_value},\r\n Model: {model},\r\n Price Range: {price_range},\r\n Processor: {Processor_value},\r\n RAM: {RAM_relation} {RAM_value},\r\n RAM Size Range: {RAM_range},\r\n Hard Disk: {HD_relation} {HD_value},\r\n Graphic Card: {hasGC},\r\n Usage: {usage} device?",noPrice)
                /*
                .Confirm((state) =>
                {
                    return new PromptAttribute($"Total for your sandwich is {cost:C2} is that ok?");
                   
                })
                */

                .Build()
                ;
        }

        
        internal static IDialog<DeviceOrder2> MakeRoot()
        {
            return Chain.From(() => new GenericQueryDialog(BuildForm));
        }


        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        public class StringTable
        {
            public string[] ColumnNames { get; set; }
            public string[,] Values { get; set; }
        }

        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {

                if (activity.Text.StartsWith("@"))
                {
                    //Add codes to connect to ML Studio
                    String id = activity.Text.Substring(1);
                    using (var client = new HttpClient())
                    {
                        var scoreRequest = new
                        {

                            Inputs = new Dictionary<string, StringTable>() {
                            {
                                "input1",
                                new StringTable()
                                {
                                    ColumnNames = new string[] {"user_id", "name", "rating"},
                                    Values = new string[,] {  { id, "", "" },  { id, "", "" },  }
                                }
                            },
                        },
                            GlobalParameters = new Dictionary<string, string>() { }
                        };
                        const string apiKey = "OW4wUkSwEv2xC2xufC5wfcbjVt5b8j4WQhQ1yMWUHlkMJoNRQJNavrViJ/InLixNzO8qc3JCxv/x8KU8iTJnJA=="; // Replace this with the API key for the web service
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                        client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/f74ff23275e641668ada8751f8fa6516/services/1ec81578b7e34ee79e6eead705de23a2/execute?api-version=2.0&details=true");
                        HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
                        if (response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            dynamic stuff = JObject.Parse(result);
                            string category = stuff.Results.output1.value.Values[0][1];
                            //Console.WriteLine("Result: {0}", result);
                            var replyActivity = activity.CreateReply("以下喺我地嘅建議：");

                            //Connect to db search result
                            if (category == "Gaming Device")
                                category = "GamingDevices";
                            if (category == "Noetbook")
                                category = "Notebook";

                            replyActivity.Attachments = new List<Attachment>();//Attachments
                            List<AIBot_FYP16004.Models.DeviceCatalog> productList = get_recommendation(category);
                            int num_of_products = productList.Count;
                            int products_to_show = num_of_products > 3 ? 3 : num_of_products;

                            //Check if the number of products returned is 0 
                            if (num_of_products == 0)
                            {
                                //message = "唔好意思，冇相關產品推薦吖";
                            }
                            else
                            {
                               // message = "以下喺我地嘅建議：";

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

                                        replyActivity.Attachments.Add(plAttachment);

                                    }
                                }
                                catch (Exception ex)
                                {
                                    //message += ("Exception Message: " + ex.Message);
                                    //message += ("Exception StackTrace: " + ex.StackTrace);
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

                                replyActivity.Attachments.Add(sCard.ToAttachment());
                            }

                            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                            await connector.Conversations.ReplyToActivityAsync(replyActivity);
                        }
                        else
                        {
                            var replyActivity = activity.CreateReply(response.StatusCode.ToString());
                            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                            await connector.Conversations.ReplyToActivityAsync(replyActivity);
/*                            Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                            // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                            Console.WriteLine(response.Headers.ToString());

                            string responseContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(responseContent);*/
                        }
                    }

                }
                else
                {
                    //Translate the Traditional Chinese to Simplified Chinese
                    var reply = string.Empty;
                    reply = await Translator.TranslateAsync(activity.Text);
                    activity.Text = reply;
                    
                    //Send the activity to be dealt with ProductQueryDialog defined in LUIS.cs
                    await Conversation.SendAsync(activity, MakeRoot);
                }
                

                //Send the activity to generic cantonese model
                //await Conversation.SendAsync(activity, () => new GenericQueryDialog());
                

                //await Conversation.SendAsync(activity, () => new GenericQueryDialog());

                /*
                var reply = string.Empty;
                reply = await Translator.TranslateAsync(activity.Text);
                //reply = activity.Text;
                var replyActivity = activity.CreateReply(reply);
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                await connector.Conversations.ReplyToActivityAsync(replyActivity);
                */
            }
            else
            {
                HandleSystemMessage(activity);
            }
            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //return response;
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
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
            for (int i = 0; i < 5; i++)
            {
                var candidates = (from product in products where product.Manufacturer.Contains(brands[i]) select product).ToList();
                int index = rnd.Next(0, candidates.Count);
                if (i == 0)
                {
                    if (index > 0)
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
    }

    /*Previous codes. Abandoned
     * 
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hello. Can I help you?");
            context.Wait(MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (this.count == 1)
            {
                this.count++;
                context.Wait(MessageReceivedAsync);
            }
            else if (message.Text.Contains("Buy") || message.Text.Contains("buy") || message.Text.Contains("purchase") || message.Text.Contains("Purchase"))
            {
                await context.PostAsync("What kind of model do you want to buy?");
                context.Wait(ProductPurchase);
            }
            else if (message.Text.Contains("Complain") || message.Text.Contains("complain") || message.Text.Contains("Complaint"))
            {
                await context.PostAsync("What kind of model do you want to complain about?");
                context.Wait(Complaint);
            }
            else
            {
                await context.PostAsync("I don't understand. Please enter again");
                context.Wait(MessageReceivedAsync);
            }
        }

        public virtual async Task ProductPurchase(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text.Contains("model") || message.Text.Contains("Model"))
            {
                await context.PostAsync("This is the link to the online store. End of conversation.");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync("Recommendation: Model XXX. Link to the online store: xxxxxx");
                context.Wait(MessageReceivedAsync);
            }
        }

        public virtual async Task Complaint(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var model = await argument;
            await context.PostAsync("Please wait a moment. We will connect the XXX team for you. End of conversation.");
            context.Wait(MessageReceivedAsync);
        }


    }
    */
}