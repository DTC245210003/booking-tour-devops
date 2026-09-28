namespace BookingTour.Models;

public static class TourStyle
{
    public static string Css(string destination) =>
        destination.Contains("Quảng Ninh") ? "cover-sea" :
        destination.Contains("Lào Cai") ? "cover-mountain" :
        destination.Contains("Đà Nẵng") ? "cover-beach" : "cover-default";

    public static string Icon(string destination) =>
        destination.Contains("Quảng Ninh") ? "\U0001F6F3\uFE0F" :
        destination.Contains("Lào Cai") ? "\U0001F3D4\uFE0F" :
        destination.Contains("Đà Nẵng") ? "\U0001F3D6\uFE0F" : "\U0001F9ED";

    public static string Image(string destination) =>
        destination.Contains("Quảng Ninh") ? "/images/halong.jpg" :
        destination.Contains("Lào Cai") ? "/images/sapa.jpg" :
        destination.Contains("Đà Nẵng") ? "/images/hoian.jpg" : "";

    public static string SeatCss(int seats) => seats == 0 ? "seat-out" : seats <= 5 ? "seat-low" : "seat-ok";
}
