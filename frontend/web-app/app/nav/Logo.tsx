'use client'
import {IoCarSport} from "react-icons/io5";
import React from "react";
import {useParamsStore} from "@/hooks/useParamsStore";
import {usePathname, useRouter} from "next/navigation";

export default function Logo() {
    const router = useRouter();
    const pathname = usePathname();
    const reset = useParamsStore(state => state.reset);

    function doReset(){
        if(pathname !== '/') router.push('/');
        reset();
    }
    return (
        <div onClick={doReset} className='cursor-pointer flex items-center gap-2 text-3xl font-semibold text-red-500'>
            <IoCarSport/>
            <div>Auctions Site</div>
        </div>
    )
}