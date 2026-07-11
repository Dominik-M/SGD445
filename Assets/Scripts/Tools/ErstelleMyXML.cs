using System.IO;
using System.Xml;
using UnityEngine;

public class ErstelleMyXML : MonoBehaviour
{
    public string Daten = "Testdaten";

    const string FILENAME = "myXml.xml";
    const string ROOTNAME = "Testeintraege";
    const string ELEMENTNAME = "Test";
    private XmlWriter xmlWriter;

    void Start()
    {
        // Dateipfad erstellen
        var path = Path.Combine(Application.persistentDataPath, FILENAME);
        // Einstellungen
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        // XML Datei anlegen
        xmlWriter = XmlWriter.Create(path, settings);
        // Prolog
        xmlWriter.WriteStartDocument();
        // Wurzelelement
        xmlWriter.WriteStartElement(ROOTNAME);
        xmlWriter.WriteElementString(ELEMENTNAME, Daten);
        xmlWriter.WriteEndElement();
        // Epilog
        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
        Debug.Log("XML-Datei erstellt: " + path);
    }
}
