import http from 'k6/http';
import { check, sleep } from 'k6';

const target_vus = 200;

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
    const res = http.get('http://localhost:5164/indicators?Code=selic&From=2025-11-01&To=2025-11-24');
    check(res, { 'status was 200': (r) => r.status == 200 });
    sleep(1);
}