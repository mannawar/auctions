'use client'
import {IoCarSport} from "react-icons/io5";
import React from "react";
import {useParamsStore} from "@/hooks/useParamsStore";

export default function Logo() {
    const reset = useParamsStore(state => state.reset);
    return (
        <div onClick={reset} className='cursor-pointer flex items-center gap-2 text-3xl font-semibold text-red-500'>
            <IoCarSport/>
            <div>Auctions Site</div>
        </div>
    )
}