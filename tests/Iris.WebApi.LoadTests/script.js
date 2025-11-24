import http from 'k6/http';
import { check, sleep } from 'k6';

const target_vus = 1500;

export const options = {
    thresholds: {
        http_req_duration: ['avg<500', 'med<500', 'min<100', 'max<2000'],
    },
    stages: [
        { duration: "30s", target: target_vus },
        { duration: "60s", target: target_vus },
        { duration: "30s", target: 0 }
    ]
};

export default function () {
    const res = http.get('http://localhost:3333/indicators?code=selic&from=2025-11-01&to=2025-11-24');
    check(res, { 'status was 200': (r) => r.status == 200 });
    sleep(1);
}