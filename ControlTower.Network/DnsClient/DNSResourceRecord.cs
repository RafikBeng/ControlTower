using System;

namespace InternetCommands.DNSRequest
{
    /// <summary>
    /// Description r�sum�e de RequestResponse.
    /// </summary>
    public class DNSResourceRecord : DNSElement
    {
        /*
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            | 0| 1| 2| 3| 4| 5| 6| 7| 8| 9|10|11|12|13|14|15|
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                                               |
            /                                               /
            /                      NAME                     /
            |                                               |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                      TYPE                     |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                     CLASS                     |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                      TTL                      |
            |                                               |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                   RDLENGTH                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
            /                     RDATA                     /
            /                                               /
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
       */


        DNSName name;
        ushort dnsType;
        DNSClass dnsClass;
        uint timeToLive;
        public DNSResourceDetails Details { get; private set; }

        #region constructeurs
        /// <summary>
        /// Construit un �l�ments de requ�te/r�ponse DNS avec la d�finition de tous les �l�ments
        /// </summary>
        /// <param name="name">Nom de la requ�te</param>
        /// <param name="dnsType">Type de requ�te/r�ponse</param>
        /// <param name="dnsClass">Classe de requ�te/r�ponse</param>
        /// <param name="timeToLive">Dur�e de vie de la r�ponse DNS</param>
        public DNSResourceRecord(DNSRequest dnsrequest, string name, ushort dnsType, DNSClass dnsClass, uint timeToLive) {
            Name = name;
            Type = dnsType;
            Class = dnsClass;
            TimeToLive = timeToLive;
        }

        /// <summary>
        /// Construit un �l�ments de requ�te/r�ponse DNS avec la d�finition de tous les �l�ments
        /// </summary>
        /// <param name="name">Nom de la requ�te</param>
        /// <param name="dnsType">Type de requ�te/r�ponse</param>
        /// <param name="dnsClass">Classe de requ�te/r�ponse</param>
        public DNSResourceRecord(DNSRequest dnsrequest, string name, ushort dnsType, DNSClass dnsClass) {
            Name = name;
            Type = dnsType;
            Class = dnsClass;
            TimeToLive = 0;
        }

        /// <summary>
        /// Construit un d�tail de ressources
        /// </summary>
        public DNSResourceRecord() {
        }

        #endregion
        #region Support de DNSRequest

        /// <summary>
        /// transforme le r�sultat de la requ�te en cha�ne de caract�res 
        /// </summary>
        /// <returns>R�sultat pour IDE</returns>
        public override string ToString() {
            System.Text.StringBuilder Ret = new System.Text.StringBuilder(256);
            Ret.Append(Details.DNSType);
            Ret.Append(",");
            Ret.Append(dnsClass);
            Ret.Append(",");
            Ret.Append(name);
            Ret.Append("\t(TTL:");
            TimeSpan tp = new TimeSpan(0, 0, (int)timeToLive);
            Ret.Append(tp.ToString());
            Ret.Append(")");
            Ret.Append("\n\t");
            Ret.Append(Details.ToString());
            
            return Ret.ToString();
             
        }

        #endregion
        #region Elements de r�ponse principaux
        /// <summary>
        /// Nom de la requ�te
        /// </summary>
        public string Name {
            get { return name.ToString(); }
            set { name = new DNSName(value); }
        }

        /// <summary>
        /// Renvoie le type de requete ou de r�ponse
        /// </summary>
        public ushort Type {
            get { return dnsType; }
            set { dnsType = value; }
        }

        /// <summary>
        /// Renvoie la classe de requ�te
        /// </summary>
        public DNSClass Class {
            get { return dnsClass; }
            set { dnsClass = value; }
        }

        /// <summary>
        /// Renvoie la dur� de vie de la r�ponse DNS
        /// </summary>
        public uint TimeToLive {
            get { return timeToLive; }
            set { timeToLive = value; }
        }

        #endregion
        
        /// <summary>
        /// ecrit vers le flux sous-jaccent
        /// </summary>
        public override void Write(DNSRequest request)
        {
            request.Write(name);
            request.Write(dnsType);
            request.Write((ushort)dnsClass);
            request.Write(TimeToLive);
            ushort additionalDatasLengthPosition = (ushort)(request.Position);
            request.Write((ushort)0);
            ushort Begin = (ushort)(request.Position);
            Details.Write(request);
            ushort End = (ushort)request.Position;
            ushort detailsLength = (ushort)(End - Begin);
            request.Write(detailsLength, additionalDatasLengthPosition);
        }

        /// <summary>
        /// Lit le flux sous-jacent
        /// </summary>
        public override void Read(DNSRequest request) {
            name = request.ReadDNSName();
            Type = request.ReadShort();
            Class = (DNSClass)request.ReadShort();
            TimeToLive = request.ReadInt();
            ushort resourceDataLength = request.ReadShort();

            request.Push();    

            Details = CreateResourceDetails(Type);
            Details.resourceRecord = this;
            Details.Read(request, resourceDataLength);

            request.Pop();
            request.Position += resourceDataLength;

        }

    }
}
