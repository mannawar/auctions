import React from "react";
import Heading from "@/app/components/Heading";
import AuctionForm from "@/app/auctions/AuctionForm";

export default function Create(){
    return (
        <div className='mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg'>
            <Heading title='Sell Your car!' subtitle='Please enter the details of your car'/>
            <AuctionForm />
        </div>
    )
}