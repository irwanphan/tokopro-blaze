using Microsoft.AspNetCore.Components;

namespace TokoProBlaze.Blazor.Components.Layout.Ribbon;

/// <summary>
/// SVG ringan untuk ribbon; konten statis (MarkupString) hanya dari string literal di repo ini.
/// </summary>
public static class RibbonIcons
{
    private static RenderFragment Svg(string paths, string sizeClass = "h-5 w-5") => builder =>
    {
        builder.AddMarkupContent(0,
            $"<svg xmlns=\"http://www.w3.org/2000/svg\" class=\"{sizeClass}\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\">{paths}</svg>");
    };

    public static readonly RenderFragment Grid = Svg("<path d=\"M4 4h6v6H4zM14 4h6v6h-6zM4 14h6v6H4zM14 14h6v6h-6z\"/>");
    public static readonly RenderFragment Tag = Svg("<path d=\"M3 5h5l10 10-5 5L3 10V5z\"/><circle cx=\"6.5\" cy=\"8.5\" r=\"1\" fill=\"currentColor\" stroke=\"none\"/>");
    public static readonly RenderFragment Folder = Svg("<path d=\"M3 6a2 2 0 012-2h4l2 2h6a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V6z\"/>");
    public static readonly RenderFragment Currency = Svg("<path d=\"M12 2v20M17 5H9.5a3.5 3.5 0 000 7h5a3.5 3.5 0 010 7H6\"/>");
    public static readonly RenderFragment Stack = Svg("<path d=\"M4 10l8 4 8-4M4 6l8 4 8-4M4 14l8 4 8-4\"/>");
    public static readonly RenderFragment Box = Svg("<path d=\"M21 8l-9-4-9 4v8l9 4 9-4V8z\"/><path d=\"M3 8l9 4 9-4M12 12v9\"/>");
    public static readonly RenderFragment PriceTag = Svg("<path d=\"M3 5h4l10 10-4 4L3 9V5z\"/><path d=\"M6.5 7.5h.01\"/>");
    public static readonly RenderFragment Warehouse = Svg("<path d=\"M3 21V10l9-4 9 4v11\"/><path d=\"M9 21V12h6v9\"/>");
    public static readonly RenderFragment WarehouseBox = Svg("<path d=\"M3 21V10l5-2 5 2v11\"/><path d=\"M13 21V13h5v8\"/><path d=\"M8 14h2v2H8z\"/>");
    public static readonly RenderFragment Truck = Svg("<path d=\"M1 3h15v11H1zM16 8h4l3 3v3h-7V8z\"/><circle cx=\"5.5\" cy=\"18.5\" r=\"2.5\"/><circle cx=\"18.5\" cy=\"18.5\" r=\"2.5\"/>");
    public static readonly RenderFragment CheckBox = Svg("<path d=\"M9 11l3 3L22 4\"/><path d=\"M21 12v7a2 2 0 01-2 2H5a2 2 0 01-2-2V5a2 2 0 012-2h11\"/>");
    public static readonly RenderFragment ArrowBox = Svg("<path d=\"M21 16V8a2 2 0 00-1-1.73l-7-4a2 2 0 00-2 0l-7 4A2 2 0 003 8v8a2 2 0 001 1.73l7 4a2 2 0 002 0l7-4A2 2 0 0021 16z\"/><path d=\"M3.27 6.96L12 12.01l8.73-5.05M12 22.08V12\"/>");
    public static readonly RenderFragment Parcels = Svg("<path d=\"M4 8l8-3 8 3v10l-8 3-8-3V8z\"/><path d=\"M12 5v15\"/>");

    public static readonly RenderFragment Users = Svg("<path d=\"M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2\"/><circle cx=\"9\" cy=\"7\" r=\"4\"/><path d=\"M23 21v-2a4 4 0 00-3-3.87M16 3.13a4 4 0 010 7.75\"/>");
    public static readonly RenderFragment Map = Svg("<polygon points=\"1 6 1 22 8 18 16 22 23 18 23 2 16 6 8 2 1 6\"/><line x1=\"8\" y1=\"2\" x2=\"8\" y2=\"18\"/><line x1=\"16\" y1=\"6\" x2=\"16\" y2=\"22\"/>");
    public static readonly RenderFragment Route = Svg("<circle cx=\"6\" cy=\"19\" r=\"3\"/><path d=\"M9 19h8.5a2.5 2.5 0 000-5H9\"/><circle cx=\"17\" cy=\"5\" r=\"3\"/>");
    public static readonly RenderFragment UserCard = Svg("<path d=\"M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2\"/><circle cx=\"12\" cy=\"7\" r=\"4\"/>");
    public static readonly RenderFragment Briefcase = Svg("<rect x=\"2\" y=\"7\" width=\"20\" height=\"14\" rx=\"2\"/><path d=\"M16 7V5a2 2 0 00-2-2h-4a2 2 0 00-2 2v2\"/>");
    public static readonly RenderFragment Clipboard = Svg("<path d=\"M16 4h2a2 2 0 012 2v14a2 2 0 01-2 2H6a2 2 0 01-2-2V6a2 2 0 012-2h2\"/><rect x=\"8\" y=\"2\" width=\"8\" height=\"4\" rx=\"1\"/>");
    public static readonly RenderFragment Invoice = Svg("<path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6M16 13H8M16 17H8M10 9H8\"/>");
    public static readonly RenderFragment Undo = Svg("<path d=\"M3 7v6h6\"/><path d=\"M21 17a9 9 0 00-15-6.7L3 13\"/>");
    public static readonly RenderFragment Spark = Svg("<path d=\"M12 2l1.5 4.5L18 8l-4.5 1.5L12 14l-1.5-4.5L6 8l4.5-1.5L12 2z\"/>");
    public static readonly RenderFragment Package = Svg("<path d=\"M16.5 9.4l-9-3.45L3 8.55v6.9l9 3.45 9-3.45v-6.9l-4.5-1.65zM3.55 8.55L12 12l8.45-3.45\"/><path d=\"M12 12v9\"/>");
    public static readonly RenderFragment Doc = Svg("<path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/>");
    public static readonly RenderFragment List = Svg("<line x1=\"8\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"8\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"8\" y1=\"18\" x2=\"21\" y2=\"18\"/><line x1=\"3\" y1=\"6\" x2=\"3.01\" y2=\"6\"/><line x1=\"3\" y1=\"12\" x2=\"3.01\" y2=\"12\"/><line x1=\"3\" y1=\"18\" x2=\"3.01\" y2=\"18\"/>");

    public static readonly RenderFragment Factory = Svg("<path d=\"M2 22h20V10l-4 4V8l-3 3V5L2 10v12z\"/><path d=\"M6 18h4v4H6zM14 16h4v6h-4z\"/>");
    public static readonly RenderFragment Inbox = Svg("<path d=\"M22 12h-6l-2 3H10L8 12H2\"/><path d=\"M5.45 5.11L2 12v6a2 2 0 002 2h16a2 2 0 002-2v-6l-3.45-6.89A2 2 0 0016.76 4H7.24a2 2 0 00-1.79 1.11z\"/>");
    public static readonly RenderFragment Outbox = Svg("<path d=\"M22 12h-6l-2-3H10L8 12H2\"/><path d=\"M5.45 5.11L2 12v6a2 2 0 002 2h16a2 2 0 002-2v-6l-3.45-6.89A2 2 0 0016.76 4H7.24a2 2 0 00-1.79 1.11z\"/>");
    public static readonly RenderFragment Cash = Svg("<rect x=\"2\" y=\"5\" width=\"20\" height=\"14\" rx=\"2\"/><path d=\"M2 10h20\"/>");
    public static readonly RenderFragment Swap = Svg("<path d=\"M16 3h5v5M4 21V16M16 3l-4 4M8 21l4-4\"/><path d=\"M8 3H3v5M20 16v5h-5M8 3l4 4M16 21l-4-4\"/>");
    public static readonly RenderFragment Chart = Svg("<path d=\"M3 3v18h18\"/><path d=\"M7 16l4-4 4 4 5-7\"/>");
    public static readonly RenderFragment Book = Svg("<path d=\"M4 19.5A2.5 2.5 0 016.5 17H20\"/><path d=\"M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z\"/>");
    public static readonly RenderFragment Lock = Svg("<rect x=\"3\" y=\"11\" width=\"18\" height=\"11\" rx=\"2\"/><path d=\"M7 11V7a5 5 0 0110 0v4\"/>");
}
