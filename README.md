# Iris
Iris is a lightweight API designed to provide seamless, low latency access to official Brazilian economic indicators, including the Selic interest rate and IPCA inflation index.

<img src="./docs/arch.png" alt="C4 Architecture Diagram">

## Local Setup
Use the following `make` commands:
```
=============================
Available commands:
=============================
down                      Stop containers
load                      Run load test
up                        Setup containers
```

## Contributing
Contributions are welcome! Open issues for bugs, questions, or suggestions. Submit pull requests with new resources or improvements.

## Further Results
After fully populating Redis, a load test was executed using k6. The results demonstrate consistent low response times and stable throughput under proposed concurrency.

<img src="./docs/k6-screenshot.png" alt="k6 Load Test Results" />