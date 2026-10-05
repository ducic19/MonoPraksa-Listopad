class Uređaj
{
 public string marka;

 public void ispis()
 {
  Console.WriteLine("ovo je uređaj");
 }
}

class Mobitel : Uređaj
{
 public string model;
 public float cijena;
 public int godizd;
 public string boja;
 private string broj;

 public Mobitel(string m, float c, int g, string b, string bb)
 {
  model = m;
  cijena = c;
  godizd = g;
  boja = b;
  broj = bb;
 }

 public string Broj
 {
  get { return broj; }
  set { broj = value; }
 }
}

class Program
 {
  static void Main(string[] args)
  {
   Uređaj mikrovalna = new Uređaj();
   mikrovalna.ispis();

   Mobitel moj = new Mobitel("Iphone 15pro", 900, 2024, "crna", "0919876754");
   moj.ispis();
   moj.boja = "crvena"; 
   Console.WriteLine(moj.boja);
   Console.WriteLine(moj.marka);
  }
 }
