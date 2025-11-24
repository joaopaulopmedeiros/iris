import http from 'k6/http';
import { check, sleep } from 'k6';

const target_vus = 1000;

export const options = {
    thresholds: {
        http_req_duration: [
            'min<10',
            'med<25',
            'p(75)<50',
            'p(95)<100',
            'p(99)<250',
            'max<500'
        ],
    },
    stages: [
        { duration: "30s", target: target_vus },
        { duration: "30s", target: target_vus },
        { duration: "30s", target: 0 }
    ]
};

export default function () {
    const res = http.get('http://localhost:3333/indicators?code=selic&from=2025-11-01&to=2025-11-24');
    check(res, { 'status was 200': (r) => r.status == 200 });
    sleep(1);
}