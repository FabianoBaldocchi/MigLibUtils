using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Uol.PagSeguro.Domain;

namespace MigLibUtils.Services.PagSeguro.BizClasses
{

    public class PGRecurrencePlan
    {
        public preApprovalData preApproval { get; set; }
        public int? maxUses { get; set; }
        public string reviewURL { get; set; }

        public class preApprovalData
        {

            private string nameField;

            private string referenceField;

            private string chargeField;

            private string periodField;

            private decimal amountPerPaymentField;

            private string cancelURLField;

            private decimal? membershipFeeField;

            private byte? trialPeriodDurationField;

            /// <remarks/>
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            public string reference
            {
                get
                {
                    return this.referenceField;
                }
                set
                {
                    this.referenceField = value;
                }
            }

            /// <remarks/>
            public string charge
            {
                get
                {
                    return this.chargeField;
                }
                set
                {
                    this.chargeField = value;
                }
            }

            /// <remarks/>
            public string period
            {
                get
                {
                    return this.periodField;
                }
                set
                {
                    this.periodField = value;
                }
            }

            /// <remarks/>
            public decimal amountPerPayment
            {
                get
                {
                    return this.amountPerPaymentField;
                }
                set
                {
                    this.amountPerPaymentField = value;
                }
            }

            /// <remarks/>
            public string cancelURL
            {
                get
                {
                    return this.cancelURLField;
                }
                set
                {
                    this.cancelURLField = value;
                }
            }

            /// <remarks/>
            public decimal? membershipFee
            {
                get
                {
                    return this.membershipFeeField;
                }
                set
                {
                    this.membershipFeeField = value;
                }
            }

            /// <remarks/>
            public byte? trialPeriodDuration
            {
                get
                {
                    return this.trialPeriodDurationField;
                }
                set
                {
                    this.trialPeriodDurationField = value;
                }
            }
        }
    }

    public class PreApprovalRequestCreatePlan
    {
        public PGRecurrencePlan preApprovalRequest { get; set; }
    }



    public class PreApprovalRequest
    {
        public string plan { get; set; }
        public string reference { get; set; }
        public Sender sender { get; set; }
        public Paymentmethod paymentMethod { get; set; }
    }

    public class Sender
    {
        public string name { get; set; }
        public string email { get; set; }
        public string ip { get; set; }
        public string hash { get; set; }
        public Phone phone { get; set; }
        public Address address { get; set; }
        public Document[] documents { get; set; }
    }

    public class Phone
    {
        public string areaCode { get; set; }
        public string number { get; set; }
    }

    public class Address
    {
        public string street { get; set; }
        public string number { get; set; }
        public string complement { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
    }

    public class Document
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Paymentmethod
    {
        public string type { get; set; }
        public Creditcard creditCard { get; set; }
    }

    public class Creditcard
    {
        public string token { get; set; }
        public Holder holder { get; set; }
    }

    public class Holder
    {
        public string name { get; set; }
        public string birthDate { get; set; }
        public Document[] documents { get; set; }
        public Billingaddress billingAddress { get; set; }
        public Phone phone { get; set; }
    }

    public class Billingaddress
    {
        public string street { get; set; }
        public string number { get; set; }
        public string complement { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
    }

    public class PreApprovalChangeMethod
    {
        public string type { get; set; }
        public Sender sender { get; set; }
        public Creditcard creditCard { get; set; }
    }


    public class PaymentOrdersResponse
    {
        public DateTime date { get; set; }
        public int resultsInThisPage { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public Dictionary<string, PaymentOrder> paymentOrders { get; set; }
    }

    public class PaymentOrder
    {
        public string code { get; set; }
        public int status { get; set; }
        public decimal amount { get; set; }
        public decimal grossAmount { get; set; }
        public DateTime lastEventDate { get; set; }
        public DateTime schedulingDate { get; set; }
        public PaymentOrderTransaction[] transactions { get; set; }
        public Discount discount { get; set; }
    }

    public class Discount
    {
        public string type { get; set; }
        public decimal value { get; set; }
    }

    public class PaymentOrderTransaction
    {
        public string code { get; set; }
        public int status { get; set; }
        public DateTime date { get; set; }

    }

    //----------------------------------


    public class PreapprovalResponse
    {
        public string name { get; set; }
        public string code { get; set; }
        public DateTime date { get; set; }
        public string tracker { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public DateTime lastEventDate { get; set; }
        public string charge { get; set; }
        public Sender sender { get; set; }
    }

}

namespace MigLibUtils.Services.PagSeguro.BizClasses.Buy
{

    // OBSERVAÇÃO: o código gerado pode exigir pelo menos .NET Framework 4.5 ou .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class payment
    {

        private string modeField;

        private string methodField;

        private paymentSender senderField;

        private string currencyField;

        private paymentItemsItem[] itemsField;

        private decimal extraAmountField;

        private string referenceField;

        private paymentShipping shippingField;

        private paymentCreditCard creditCardField;

        /// <remarks/>
        public string mode
        {
            get
            {
                return this.modeField;
            }
            set
            {
                this.modeField = value;
            }
        }

        /// <remarks/>
        public string method
        {
            get
            {
                return this.methodField;
            }
            set
            {
                this.methodField = value;
            }
        }

        /// <remarks/>
        public paymentSender sender
        {
            get
            {
                return this.senderField;
            }
            set
            {
                this.senderField = value;
            }
        }

        /// <remarks/>
        public string currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute("item", IsNullable = false)]
        public paymentItemsItem[] items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        [XmlIgnore]
        public decimal extraAmount
        {
            get
            {
                return this.extraAmountField;
            }
            set
            {
                this.extraAmountField = value;
            }
        }
        [XmlElement("extraAmount")]
        public string extraAmountString { get { return extraAmount.ToString("0.00").Replace(",", "."); } set { if (decimal.TryParse(value, out decimal v)) { extraAmount = v; } } }

        /// <remarks/>
        public string reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }

        /// <remarks/>
        public paymentShipping shipping
        {
            get
            {
                return this.shippingField;
            }
            set
            {
                this.shippingField = value;
            }
        }

        /// <remarks/>
        public paymentCreditCard creditCard
        {
            get
            {
                return this.creditCardField;
            }
            set
            {
                this.creditCardField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentSender
    {

        private string nameField;

        private string emailField;

        private paymentSenderPhone phoneField;

        private paymentSenderDocuments documentsField;

        private string hashField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public paymentSenderPhone phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        public paymentSenderDocuments documents
        {
            get
            {
                return this.documentsField;
            }
            set
            {
                this.documentsField = value;
            }
        }

        /// <remarks/>
        public string hash
        {
            get
            {
                return this.hashField;
            }
            set
            {
                this.hashField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentSenderPhone
    {

        private string areaCodeField;

        private string numberField;

        /// <remarks/>
        public string areaCode
        {
            get
            {
                return this.areaCodeField;
            }
            set
            {
                this.areaCodeField = value;
            }
        }

        /// <remarks/>
        public string number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentSenderDocuments
    {

        private paymentSenderDocumentsDocument documentField;

        /// <remarks/>
        public paymentSenderDocumentsDocument document
        {
            get
            {
                return this.documentField;
            }
            set
            {
                this.documentField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentSenderDocumentsDocument
    {

        private string typeField;

        private string valueField;

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }


    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentItemsItem
    {

        private int idField;

        private string descriptionField;

        private int quantityField;

        private decimal amountField;

        /// <remarks/>
        public int id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public int quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        [XmlIgnore]
        public decimal amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
        [XmlElement("amount")]
        public string amountString { get { return amount.ToString("0.00").Replace(",", "."); } set { if (decimal.TryParse(value, out decimal v)) { amount = v; } } }

    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentShipping
    {

        private bool addressRequiredField;

        /// <remarks/>
        public bool addressRequired
        {
            get
            {
                return this.addressRequiredField;
            }
            set
            {
                this.addressRequiredField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentCreditCard
    {

        private string tokenField;

        private paymentCreditCardInstallment installmentField;

        private paymentCreditCardHolder holderField;

        private paymentCreditCardBillingAddress billingAddressField;

        /// <remarks/>
        public string token
        {
            get
            {
                return this.tokenField;
            }
            set
            {
                this.tokenField = value;
            }
        }

        public paymentCreditCardInstallment installment
        {
            get
            {
                return this.installmentField;
            }
            set
            {
                this.installmentField = value;
            }
        }

        /// <remarks/>
        public paymentCreditCardHolder holder
        {
            get
            {
                return this.holderField;
            }
            set
            {
                this.holderField = value;
            }
        }

        /// <remarks/>
        public paymentCreditCardBillingAddress billingAddress
        {
            get
            {
                return this.billingAddressField;
            }
            set
            {
                this.billingAddressField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentCreditCardInstallment
    {

        private int quantityField;

        private decimal valueField;

        /// <remarks/>
        public int quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        [XmlIgnore]
        public decimal value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
        [XmlElement("value")]
        public string valueString { get { return value.ToString("0.00").Replace(",", "."); } set { if (decimal.TryParse(value, out decimal v)) { valueField = v; } } }

    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentCreditCardHolder
    {

        private string nameField;

        private paymentCreditCardHolderDocuments documentsField;

        private string birthDateField;

        private paymentCreditCardHolderPhone phoneField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public paymentCreditCardHolderDocuments documents
        {
            get
            {
                return this.documentsField;
            }
            set
            {
                this.documentsField = value;
            }
        }

        /// <remarks/>
        public string birthDate
        {
            get
            {
                return this.birthDateField;
            }
            set
            {
                this.birthDateField = value;
            }
        }

        /// <remarks/>
        public paymentCreditCardHolderPhone phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentCreditCardHolderDocuments
    {

        private paymentCreditCardHolderDocumentsDocument documentField;

        /// <remarks/>
        public paymentCreditCardHolderDocumentsDocument document
        {
            get
            {
                return this.documentField;
            }
            set
            {
                this.documentField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentCreditCardHolderDocumentsDocument
    {

        private string typeField;

        private string valueField;

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentCreditCardHolderPhone
    {

        private string areaCodeField;

        private string numberField;

        /// <remarks/>
        public string areaCode
        {
            get
            {
                return this.areaCodeField;
            }
            set
            {
                this.areaCodeField = value;
            }
        }

        /// <remarks/>
        public string number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class paymentCreditCardBillingAddress
    {

        private string streetField;

        private string numberField;

        private string complementField;

        private string districtField;

        private string cityField;

        private string stateField;

        private string countryField;

        private string postalCodeField;

        /// <remarks/>
        public string street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
            }
        }

        /// <remarks/>
        public string number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        public string complement
        {
            get
            {
                return this.complementField;
            }
            set
            {
                this.complementField = value;
            }
        }

        /// <remarks/>
        public string district
        {
            get
            {
                return this.districtField;
            }
            set
            {
                this.districtField = value;
            }
        }

        /// <remarks/>
        public string city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string postalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }
    }


}
