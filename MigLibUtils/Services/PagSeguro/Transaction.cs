using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MigLibUtils.Services.PagSeguro.BizClasses
{


    // OBSERVAÇÃO: o código gerado pode exigir pelo menos .NET Framework 4.5 ou .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class transaction
    {
        public enum transactionstatus { AGUARDANDO_PAGAMENTO = 1, EM_ANALISE = 2, PAGA = 3, DISPONIVEL = 4, EM_DISPUTA = 5, DEVOLVIDA = 6, CANCELADA = 7 };

        private System.DateTime dateField;

        private string codeField;

        private string referenceField;

        private byte typeField;

        private transactionstatus statusField;

        private System.DateTime lastEventDateField;

        private transactionPaymentMethod paymentMethodField;

        private string paymentLinkField;

        private decimal grossAmountField;

        private decimal discountAmountField;

        private transactionCreditorFees creditorFeesField;

        private decimal netAmountField;

        private decimal extraAmountField;

        private byte installmentCountField;

        private transactionItem[] itemsField;

        private transactionSender senderField;

        private transactionPrimaryReceiver primaryReceiverField;

        /// <remarks/>
        public System.DateTime date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
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
        public byte type
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
        public byte status
        {
            get
            {
                return (byte)this.statusField;
            }
            set
            {
                this.statusField = (transactionstatus)value;
            }
        }

        public transactionstatus EnumStatus
        {
            get
            {
                return this.statusField;
            }

        }

        /// <remarks/>
        public System.DateTime lastEventDate
        {
            get
            {
                return this.lastEventDateField;
            }
            set
            {
                this.lastEventDateField = value;
            }
        }

        /// <remarks/>
        public transactionPaymentMethod paymentMethod
        {
            get
            {
                return this.paymentMethodField;
            }
            set
            {
                this.paymentMethodField = value;
            }
        }

        /// <remarks/>
        public string paymentLink
        {
            get
            {
                return this.paymentLinkField;
            }
            set
            {
                this.paymentLinkField = value;
            }
        }



        public decimal grossAmount
        {
            get
            {
                return this.grossAmountField;
            }
            set
            {
                this.grossAmountField = value;
            }
        }

        public decimal discountAmount
        {
            get
            {
                return this.discountAmountField;
            }
            set
            {
                this.discountAmountField = value;
            }
        }



        /// <remarks/>
        public transactionCreditorFees creditorFees
        {
            get
            {
                return this.creditorFeesField;
            }
            set
            {
                this.creditorFeesField = value;
            }
        }


        public decimal netAmount
        {
            get
            {
                return this.netAmountField;
            }
            set
            {
                this.netAmountField = value;
            }
        }


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

        /// <remarks/>
        public byte installmentCount
        {
            get
            {
                return this.installmentCountField;
            }
            set
            {
                this.installmentCountField = value;
            }
        }

        /// <remarks/>
        public int itemCount
        {
            get
            {
                return this.items.Length;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("item", IsNullable = false)]
        public transactionItem[] items
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

        /// <remarks/>
        public transactionSender sender
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
        public transactionPrimaryReceiver primaryReceiver
        {
            get
            {
                return this.primaryReceiverField;
            }
            set
            {
                this.primaryReceiverField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class transactionPaymentMethod
    {

        private byte typeField;

        private byte codeField;

        /// <remarks/>
        public byte type
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
        public byte code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class transactionCreditorFees
    {

        private decimal operationalFeeAmountField;

        private decimal intermediationRateAmountField;

        private decimal intermediationFeeAmountField;


        public decimal operationalFeeAmount
        {
            get
            {
                return this.operationalFeeAmountField;
            }
            set
            {
                this.operationalFeeAmountField = value;
            }
        }

  
        public decimal intermediationRateAmount
        {
            get
            {
                return this.intermediationRateAmountField;
            }
            set
            {
                this.intermediationRateAmountField = value;
            }
        }

    
        public decimal intermediationFeeAmount
        {
            get
            {
                return this.intermediationFeeAmountField;
            }
            set
            {
                this.intermediationFeeAmountField = value;
            }
        }

    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class transactionItem
    {

        private byte idField;

        private string descriptionField;

        private byte quantityField;

        private decimal amountField;

        /// <remarks/>
        public byte id
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
        public byte quantity
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


    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class transactionSender
    {

        private string nameField;

        private string emailField;

        private transactionSenderPhone phoneField;

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
        public transactionSenderPhone phone
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
    public partial class transactionSenderPhone
    {

        private byte areaCodeField;

        private uint numberField;

        /// <remarks/>
        public byte areaCode
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
        public uint number
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
    public partial class transactionPrimaryReceiver
    {

        private string publicKeyField;

        /// <remarks/>
        public string publicKey
        {
            get
            {
                return this.publicKeyField;
            }
            set
            {
                this.publicKeyField = value;
            }
        }
    }
}

