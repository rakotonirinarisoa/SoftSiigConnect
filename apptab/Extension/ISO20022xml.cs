using System.Xml;
using System.Xml.Linq;

namespace apptab.Extension
{
    public class ISO20022xml
    {
        public XmlDocument Fichier { get; set; }
        public string Chemin { get; set; }
        public string NomFichier { get; set; }
    }
}
