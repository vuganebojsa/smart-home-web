import { Component, OnInit } from '@angular/core';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit{
  chart!: Chart;
  tooltipData = [];
  tooltipNonRealTimeData = [];
  ngOnInit(): void {

    this.InitBarChart();
  }

  private InitBarChart() {

    this.chart = new Chart('barChart', {
      type: 'line',
      data: {
        labels: [],
        datasets: [{
          label: 'Power Consumed',
          data: [],
          backgroundColor: ['#01ACAB'],
          borderColor: [
            'rgba(0, 0, 0, 1)'
          ],
          borderWidth: 1
        }]
      },
      options: {
        maintainAspectRatio: false,
        scales: {
          y: {
            beginAtZero: true
          },
          x: {
            ticks: {
              autoSkip: true,
              maxTicksLimit: 3,
            },
          }

        },
        plugins: {
          tooltip: {
            callbacks: {
              title: function (tooltipItem) {
                return tooltipItem[0].label;
              },
              label:  (tooltipItem) => {
                const index = tooltipItem.dataIndex;
                return `The system consumed: ${this.tooltipNonRealTimeData[index].consumedValues} KWH`;
              }
            }
          }
        }
      }
    });
  }

}
