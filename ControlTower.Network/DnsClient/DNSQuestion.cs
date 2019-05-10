using System;
using System.Text;

namespace InternetCommands.DNSRequest
{
    /// <summary>
    /// Question envoy�e au serveur.
    /// </summary>
    public class DNSQuestion : DNSElement
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
       */


        DNSName name;
        ushort dnsType;
        DNSClass dnsClass;

        /// <summary>
        /// Construit un �l�ments de requ�te/r�ponse DNS avec la d�finition de tous les �l�ments
        /// </summary>
        /// <param name="name">Nom de la requ�te</param>
        /// <param name="dnsType">Type de requ�te/r�ponse</param>
        /// <param name="dnsClass">Classe de requ�te/r�ponse</param>
        public DNSQuestion(string name, ushort dnsType, DNSClass dnsClass) {
            Name = name;
            Type = dnsType;
            Class = dnsClass;
        }

        /// <summary>
        /// Construit un �l�ments de requ�te/r�ponse DNS avec la d�finition de tous les �l�ments
        /// </summary>
        /// <param name="name">Nom de la requ�te</param>
        /// <param name="dnsType">Type de requ�te/r�ponse</param>
        /// <param name="dnsClass">Classe de requ�te/r�ponse</param>
        public DNSQuestion(string name, string dnsType, DNSClass dnsClass) {
            Name = name;
            switch (dnsType) {
            case "ALL":
                Type = 0xFF;
                break;
            case "AXFR":
                Type = 0xFC;
                break;
            case "MAILB":
                Type = 0xFD;
                break;
            case "MAILA":
                Type = 0xFE;
                break;
            default:
                Type = (ushort)ResourceDetailsLoader.Invoke(DNSServiceName[dnsType],"Service");
                break;
            }
            Class = dnsClass;
        }

        /// <summary>
        /// Cr�� une question DNS
        /// </summary>
        public DNSQuestion() {
        }

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

        public override string ToString() {
            StringBuilder ret = new StringBuilder();
            switch (Type) {
            case 0xFF:
                ret.Append ("ALL");
                break;
            case 0xFC:
                ret.Append ("AXFR");
                break;
            case 0xFD:
                ret.Append ("MAILB");
                break;
            case 0xFE:
                ret.Append ("MAILA");
                break;
            default:
                ret.Append ((string)ResourceDetailsLoader.Invoke(DNSServiceNumber[dnsType], "Name"));
                break;
            }
            ret.Append(",");
            ret.Append(Class.ToString());
            ret.Append("\t");
            ret.Append(Name);
            return ret.ToString();
        }

        /// <summary>
        /// ecrit vers le flux sous-jaccent
        /// </summary>
        public override void Write(DNSRequest request)
        {

            request.Write(name);
            request.Write(dnsType);
            request.Write((ushort)dnsClass);
        }

        /// <summary>
        /// Lit le flux sous-jacent
        /// </summary>
        public override void Read(DNSRequest request) {
            name = request.ReadDNSName();
            Type = request.ReadShort();
            Class = (DNSClass)request.ReadShort();
        }

    }
}
