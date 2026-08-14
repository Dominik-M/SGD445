[System.Serializable]
public class Gegenstand
{
    public string name;
    public string bedingung;
    public int anzahl;
    public int maxAnzahl;

    public override string ToString()
    {
        if (anzahl > 1)
            return name + "\nx" + anzahl.ToString();
        else return name;
    }

    public bool Equals(Gegenstand other)
    {
        return name.Equals(other.name);
    }
}