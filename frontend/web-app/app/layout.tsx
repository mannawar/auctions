import type { Metadata } from 'next'
import './globals.css'
import Navbar from "@/app/nav/Navbar";
import React from "react";
import {Toaster} from "react-hot-toast";
import ToasterProvider from "@/app/providers/ToasterProvider";


export const metadata: Metadata = {
  title: 'Auction Site',
  description: 'Generated by create next app',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body>
      <ToasterProvider />
      <Navbar/>
      <main className='container mx-auto px-5 pt-10'>
          {children}
      </main>
      </body>
    </html>
  )
}
