import { useState,useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getVenues } from "./api";
import type { Venue } from "./types";
import { useParams } from "react-router-dom";
import VenueCard from "./VenueCard";

export default function VenueList(){

    const[venues,SetVenues] = useState<Venue[]>([]);
    const navigate = useNavigate();
    const {city} = useParams();

    useEffect(() => {
        getVenues().then(SetVenues);
    }, []);
    return (
        <div>
            <h1>Smestaji u {city}</h1>
            <div className="bookings-grid">
                {venues.map(v =>(
                    <VenueCard key = {v.id} venue={v}
                      onClick={() => navigate(`/venues/${v.id}`)}/>

                ))}
            </div>
        </div>
    );
}





