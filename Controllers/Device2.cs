using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using System.Text;

namespace AIBot_FYP16004
{

    public enum PreferenceOptions
    {
        Unknown,
        [Terms("yes")]
        yes,
        [Terms("no")]
        no
    };

    //Brands available
    public enum BrandOptions
    {
        Unknown,
        [Terms("Microsoft", "microsoft")]
        Microsoft,
        [Terms("Lenovo", "lenovo")]
        Lenovo,
        [Terms("ASUS", "asus")]
        ASUS,
        [Terms("HP","hp")]
        HP,
        [Terms("Samsung")]
        Samsung,
        [Terms("Fujitsu")]
        Fujitsu,
        [Terms("Acer")]
        Acer,
        [Terms("Dell")]
        Dell
    };

    //Price ranges to be picked by users
    public enum PriceRangeOptions
    {
        Unknown,
        [Terms("5000hkd-8000hkd")]
        [Describe("5000hkd-8000hkd")]
        Range1,
        [Terms("8000hkd-12000hkd")]
        [Describe("8000hkd-12000hkd")]
        Range2,
        [Terms("12000hkd-15000hkd")]
        [Describe("12000hkd-15000hkd")]
        Range3,
        [Terms("above 15000hkd")]
        [Describe("above 15000hkd")]
        Range4
    };

    //Device Types
    public enum DeviceTypeOptions
    {
        Unknown,
        [Terms("twoInOne")]
        [Describe("twoInOne")]
        twoInOne,
        [Terms("Desktop")]
        [Describe("Desktop")]
        Desktop,
        [Terms("GamingDevices")]
        [Describe("GamingDevices")]
        GamingDevices,
        [Terms("Notebook")]
        [Describe("Notebook")]
        Notebook,
        [Terms("Tablet")]
        [Describe("Tablet")]
        Tablet
    };

    //This class represents the user requirements on the device
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "唔好意思，我唔明 \"{0}\".", "試多一次喇, 我唔明 \"{0}\".")] //Add templates to respond to unknown text
    public class DeviceOrder2
    {

        [Optional]
        [Prompt("你對設備牌子有冇要求? {||}")]
        [Describe("牌子")]
        public BrandOptions Brand_value;

        [Optional]
        [Prompt("你對價格有冇要求? {||}")]
        [Describe("價格")]
        public PriceRangeOptions price_range;

        [Optional]
        [Prompt("你對設備類型有冇要求? {||}")]
        [Describe("設備類型")]
        public DeviceTypeOptions type_value;

        //The string representation of the class
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Brand: {0},", Brand_value);

            string description="";
            switch (price_range)
            {
                case PriceRangeOptions.Range1:
                    description= "2000hkd~5000hkd";
                    break;
                case PriceRangeOptions.Range2:
                    description = "5000hkd~8000hkd";
                    break;
                case PriceRangeOptions.Range3:
                    description = "8000hkd~10000hkd";
                    break;
                case PriceRangeOptions.Range4:
                    description = "above 10000hkd";
                    break;
            }
            builder.AppendFormat(" Price Range: {0}", description);
            builder.AppendFormat(" Device Type: {0}", type_value);
            return builder.ToString();
        }

    };
}