# Contracts

Get selic values within a given time range
```
GET /v1/selic?startDate=2025-01-01&endDate=2025-10-01

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

Get selic stats within a given time range
```
GET /v1/selic/stats?startDate=2025-01-01&endDate=2025-10-01

{
  "avg": 8.25,
  "min": 2.00,
  "max": 13.75,
  "stdDev": 3.12
}
```