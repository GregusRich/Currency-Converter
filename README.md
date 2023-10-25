# Currency-Converter
A mobile app prototype that converts AUD to other currencies. This is using an API call for live exchange rates. 

## Currency Converter Mobile App
A mobile app prototype designed to convert USD into various other currencies. The conversion rates are fetched in real-time using an API to ensure accuracy.

## Important Note 🚨
This app leverages a free API for currency conversion. Due to the limitations with this free-tier service, there might be restrictions on the number of API calls in a given time frame. Excessive calls might result in temporary blocking.

## Features
- Converts USD to multiple foreign currencies.
- Uses a live exchange rate API for real-time data (very limited use and will block if used often).
- User-friendly interface with a numeric keypad.
- MVVM design pattern

## During testing, it's recommended to minimise the number of live API calls. 

The recommended approach is:
1. Make an API call and get the response (JSON file).
2. Store this response locally within your code.
3. Utilise this stored response for testing and development.
