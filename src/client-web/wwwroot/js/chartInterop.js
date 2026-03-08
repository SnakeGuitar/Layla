function createBarChart(canvasId, newUsersPerMonth) {
  const canvas = document.getElementById(canvasId);
  if (!canvas) {
    console.error(`Canvas Chart element with id "${canvasId}" not found.`);
    return;
  }
  try {
    new Chart(canvas, {
      type: "line",
      data: {
        labels: [
          "January",
          "February",
          "March",
          "April",
          "May",
          "June",
          "July",
          "August",
          "September",
          "October",
          "November",
          "December",
        ],
        datasets: [
          {
            label: "New Users",
            data: newUsersPerMonth,
            backgroundColor: "rgb(254, 154, 0)",
            borderColor: "rgb(254, 154, 0)",
            borderWidth: 1,
          },
        ],
      },
      options: {
        responsive: true,
        plugins: {
          legend: { position: "top" },
          title: { display: true, text: "Subscripciones mensuales" },
        },
        scales: {
          y: { beginAtZero: true },
        },
      },
    });
  } catch (error) {
    console.error(
      `Error creating Canvas Chart element with id "${canvasId}": ${error}`,
    );
  }
}
window.createBarChart = createBarChart;
