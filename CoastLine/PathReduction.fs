namespace CoastLine

open System

type Point = { Long: double; Lat : double}

module Reduce = 

    let private findPerpendicularDistance p p1 p2 =
        if (p1.Long = p2.Long) then
            Math.Abs(p.Long - p1.Long)
        else 
            let slope = (p2.Lat - p1.Lat) / (p2.Long - p1.Long)
            let intercept = p1.Lat - (slope * p1.Long)
            Math.Abs(slope * p.Long - p.Lat + intercept) / Math.Sqrt(Math.Pow(slope, 2.) + 1.)

    let rec Reduce epsilon (points : Point[]) =
        if points.Length < 3 || epsilon = 0. then
            points
        else
            let firstPoint = points.[0]
            let lastPoint = points.[points.Length - 1]

            let mutable index = -1
            let mutable dist = 0.0

            for i in 1..points.Length-1 do
                let cDist = findPerpendicularDistance points.[i] firstPoint lastPoint
                if (cDist > dist) then
                    dist <- cDist
                    index <- i
        
            if (dist > epsilon) then
                let l1 = points.[0..index]
                let l2 = points.[index..]
                let r1 = Reduce epsilon l1
                let r2 = Reduce epsilon l2
                Array.append (r1.[0..r1.Length-2]) r2 
            else
                [|firstPoint; lastPoint|]

