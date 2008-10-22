<map version="0.9.0_Beta_8">
<!-- To view this file, download free mind mapping software FreeMind from http://freemind.sourceforge.net -->
<node CREATED="1186440620328" ID="Freemind_Link_1696776997" MODIFIED="1186440715015" TEXT="Nando">
<icon BUILTIN="full-1"/>
<node CREATED="1186440730171" HGAP="25" ID="_" MODIFIED="1186440941000" POSITION="right" TEXT="Web" VSHIFT="-53">
<node CREATED="1186440942453" ID="Freemind_Link_776243531" MODIFIED="1186440952421" TEXT="consente di vedere e modificare tutti i cataloghi">
<node COLOR="#ff0000" CREATED="1186440958421" ID="Freemind_Link_81288512" MODIFIED="1186496837015" TEXT="deve essere possibile raggruppare un tot di file (per esempio 2 tempi di un film)"/>
<node CREATED="1186440997890" ID="Freemind_Link_180208328" MODIFIED="1186441006031" TEXT="cancellare una serie di file"/>
<node CREATED="1186441011000" ID="Freemind_Link_1834444746" MODIFIED="1186441018781" TEXT="inserire manualmente"/>
<node CREATED="1218450424859" ID="Freemind_Link_783392624" MODIFIED="1218450441343" TEXT="Catalog, Entry e File sono entit&#xe0; taggabili"/>
</node>
</node>
<node CREATED="1186440740812" ID="Freemind_Link_1122565052" MODIFIED="1186440744218" POSITION="right" TEXT="Client">
<node CREATED="1186440751093" HGAP="40" ID="Freemind_Link_721228432" MODIFIED="1186440787156" TEXT="serve sostanzialmente per poter accedere ai dati...&#xa;quindi per poter inserire nuovi cd" VSHIFT="28"/>
<node CREATED="1186440790046" ID="Freemind_Link_403799467" MODIFIED="1186440793578" TEXT="Scansiona">
<node CREATED="1186440795281" ID="Freemind_Link_1691852486" MODIFIED="1186440812375" TEXT="scandisce dalla root">
<node CREATED="1186440813578" ID="Freemind_Link_304742243" MODIFIED="1186440836703" TEXT="se trova una cartella VIDEO_TS la considera un dvd video"/>
<node CREATED="1186440840234" ID="Freemind_Link_1401364068" MODIFIED="1186440856843" TEXT="se trova solo audio e m3u parsa tutto"/>
<node CREATED="1186440897328" ID="Freemind_Link_1563858629" MODIFIED="1186440918937" TEXT="se trova solo video (o sottotitoli) parsa tutto"/>
<node CREATED="1186440870828" ID="Freemind_Link_1862656763" MODIFIED="1186440894671" TEXT="se c&apos;&#xe8; una cartella con contenuto misto niente"/>
</node>
<node CREATED="1186441039921" ID="Freemind_Link_1607878796" MODIFIED="1186441044125" TEXT="inserimento manuale">
<node CREATED="1186441045718" ID="Freemind_Link_1789764472" MODIFIED="1186441066390" TEXT="scegli i file da raggruppare e dagli una descrizione"/>
</node>
</node>
<node CREATED="1186441225546" ID="Freemind_Link_36625049" MODIFIED="1186441244937" TEXT="Dopo la scansione inserisce in un db locale"/>
<node CREATED="1186441250375" ID="Freemind_Link_209167331" MODIFIED="1186441263484" TEXT="Si sincronizza con un db remoto">
<node CREATED="1186441339062" ID="Freemind_Link_1856472696" MODIFIED="1186441370968" TEXT="Ogni modifica ha un datetime, sincronizzo solo nei delta tra le ultime modifiche"/>
<node CREATED="1186496608734" ID="Freemind_Link_1999608711" MODIFIED="1186496625046" TEXT="La sincronizzazione avviene inviando file XML con tutte le modifiche">
<node CREATED="1186496634265" ID="Freemind_Link_609155848" MODIFIED="1186496655250" TEXT="chiedi data ultima modificha"/>
<node CREATED="1186496656468" ID="Freemind_Link_169163754" MODIFIED="1186496669781" TEXT="invia xml con tutte le modifiche &gt;= data"/>
</node>
</node>
</node>
</node>
</map>
