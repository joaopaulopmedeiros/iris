import http from 'k6/http';
import { check, sleep } from 'k6';

const target_vus = __ENV.TARGET_VUS ? parseInt(__ENV.TARGET_VUS) : 1000;

export const options = {
    thresholds: {
        http_req_duration: [
            'min<10',
            'med<25',
            'p(75)<50',
            'p(95)<100',
            'p(99)<300',
            'p(99.9)<500',
        ],
    },
    stages: [
        { duration: "30s", target: target_vus },
        { duration: "30s", target: target_vus },
        { duration: "30s", target: 0 }
    ]
};

const dateRanges = [
    { from: '2025-01-01', to: '2025-01-30' },
    { from: '2025-02-01', to: '2025-03-02' },
    { from: '2025-03-15', to: '2025-04-14' },
    { from: '2025-04-01', to: '2025-04-30' },
    { from: '2025-05-10', to: '2025-06-09' },
    { from: '2025-06-01', to: '2025-06-30' },
    { from: '2025-07-15', to: '2025-08-14' },
    { from: '2025-08-01', to: '2025-08-30' },
    { from: '2025-09-05', to: '2025-10-05' },
    { from: '2025-10-01', to: '2025-10-30' },
    { from: '2025-11-01', to: '2025-11-30' },
    { from: '2025-12-01', to: '2025-12-30' },
    { from: '2024-06-01', to: '2024-06-30' },
    { from: '2024-09-01', to: '2024-09-30' },
    { from: '2024-12-01', to: '2024-12-30' },
];

const codes = ['selic', 'ipca'];

export default function () {
    const range = dateRanges[Math.floor(Math.random() * dateRanges.length)];
    const code = codes[Math.floor(Math.random() * codes.length)];

    const url = `http://localhost:3333/indicators?code=${code}&from=${range.from}&to=${range.to}`;
    const res = http.get(url);

    check(res, {
        'status was 200': (r) => r.status == 200
    });

    sleep(1);
}