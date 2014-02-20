module GreatCircle

[<AutoOpen>]
module Constants =

    let earthRadius = 6371.

module GreatCircle = 

    open System

    let degToRad (degrees : float) =
        degrees * Math.PI / 180.

    /// Calculates the great-circle distance between two Latitude/Longitude positions on a sphere of given radius.
    let DistanceBetween (radius:float) lat1 long1 lat2 long2 =
        let lat1r, lat2r, long1r, long2r = lat1 |> degToRad, 
                                           lat2 |> degToRad,
                                           long1 |> degToRad,
                                           long2 |> degToRad
        let deltaLat = lat2r - lat1r
        let deltaLong = long2r - long1r

        let a = Math.Sin(deltaLat/2.) ** 2. +
                (Math.Sin(deltaLong/2.) ** 2. * Math.Cos((double)lat1r) * Math.Cos((double)lat2r))

        let c = 2. * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.-a))

        radius * c

    /// Calculate DistanceBetween for Earth.
    let DistanceBetweenEarth = DistanceBetween earthRadius

    let QuickDist x1 y1 x2 y2 =
        ((x1 - x2) ** 2.) + ((y1 - y2) ** 2.) |> sqrt
