# Contracts

Get selic values within a given time range
```
GET /v1/indicators?code=selic&from=2025-01-01&to=2025-10-01

[
    {
        "value": "10.65",
        "referenceDate": "2025-10-01"
    },
    {
        "value": "9.23",
        "referenceDate": "2025-09-01"
    }    
]
```
