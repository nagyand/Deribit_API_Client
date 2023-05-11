# Deribit_API_Client

Hiányzó részek:

DeribitAPIHandler osztálynak implementálni kéne a IDisposeble interfészt vagy tartalmaznia kéne egy Close metódust, hogy le tudjuk zárni a WebSokcetet rendesen. 

WebsocketAPIClient-nél A hibakezelés és a retry is kimaradt az implementálásból

A leiratkozás kezelése is kimaradt

A Request modellek
(Az ChannelsSubscriptionRequest segítségével íratkoztunk volna fel a megfelelő csatornákra, A AuthenticationRequest segítségével tudunk bejelentkezni)

A konfiguráció validálása

Unit tesztek

# Alternatív út
A jelenlegi WebsocketAPIClient helyett a SignalR használata
