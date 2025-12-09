using System.IO;
using Newtonsoft.Json;

namespace lab3;

public class ToJSON
{
    public static void Transfer(Model model)
    {
        var dataList = new List<Point>();
        
        foreach (var p in model.CurvePoints)
        {
            dataList.Add(new Point(p.XToSystem(),p.YToSystem(),p.PointName));
        }
        
        string json = JsonConvert.SerializeObject(dataList, Formatting.Indented);
        string filePath = @"C:\Programming\Geo_Modelling\curveData.json";
        
        File.WriteAllText(filePath, json);
    }
}