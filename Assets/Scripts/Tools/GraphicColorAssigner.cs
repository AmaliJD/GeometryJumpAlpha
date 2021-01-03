using UnityEngine;
using UnityEngine.UI;

public class GraphicColorAssigner : ColorAssigner
{
    protected override void AssignColor(Color color, float hue, float sat, float val, float alpha)
    {
        if (this == null) { return; }

        float h = 0, s = 0, v = 0, a = color.a;

        Color.RGBToHSV(color, out h, out s, out v);

        h += (hue / 360);
        s += sat;
        v += val;
        a += alpha;

        if (h > 1) { h -= 1; }
        else if (h < 0) { h += 1; }

        if (s > 1) { s = 1; }
        else if (s < 0) { s = 0; }

        if (v > 1) { v = 1; }
        else if (v < 0) { v = 0; }

        if (a > 1) { a = 1; }
        else if (a < 0) { a = 0; }

        color = Color.HSVToRGB(h, s, v);
        color.a = a;

        GetComponent<Graphic>().color = color;
    }
}