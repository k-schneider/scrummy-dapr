window.scrummyJS = {

  drawChart: (elementId, votes) => {
    // Generated palette using: http://phrogz.net/css/distinct-colors.html
    const palette = [
      'rgba(178,0,0,0.4)', 'rgba(64,45,32,0.4)', 'rgba(143,153,0,0.4)', 'rgba(0,242,65,0.4)', 'rgba(0,102,153,0.4)', 'rgba(110,105,140,0.4)', 'rgba(140,105,129,0.4)', 'rgba(242,61,61,0.4)', 'rgba(128,108,96,0.4)', 'rgba(61,64,16,0.4)',
      'rgba(127,255,161,0.4)', 'rgba(0,43,64,0.4)', 'rgba(68,0,255,0.4)', 'rgba(115,0,61,0.4)', 'rgba(140,35,35,0.4)', 'rgba(140,75,0,0.4)', 'rgba(234,242,121,0.4)', 'rgba(182,242,198,0.4)', 'rgba(182,222,242,0.4)', 'rgba(97,77,153,0.4)',
      'rgba(242,61,157,0.4)', 'rgba(89,22,22,0.4)', 'rgba(89,48,0,0.4)', 'rgba(201,204,153,0.4)', 'rgba(48,64,52,0.4)', 'rgba(105,129,140,0.4)', 'rgba(198,182,242,0.4)', 'rgba(51,26,39,0.4)', 'rgba(166,83,83,0.4)', 'rgba(217,141,54,0.4)',
      'rgba(138,140,105,0.4)', 'rgba(0,153,102,0.4)', 'rgba(38,47,51,0.4)', 'rgba(76,67,89,0.4)', 'rgba(255,0,102,0.4)', 'rgba(242,182,182,0.4)', 'rgba(217,166,108,0.4)', 'rgba(82,102,0,0.4)', 'rgba(0,64,43,0.4)', 'rgba(0,95,179,0.4)',
      'rgba(27,0,51,0.4)', 'rgba(89,0,36,0.4)', 'rgba(102,77,77,0.4)', 'rgba(153,117,77,0.4)', 'rgba(110,166,0,0.4)', 'rgba(96,191,159,0.4)', 'rgba(96,147,191,0.4)', 'rgba(186,121,242,0.4)', 'rgba(255,191,217,0.4)', 'rgba(255,34,0,0.4)',
      'rgba(255,225,191,0.4)', 'rgba(149,179,89,0.4)', 'rgba(115,153,140,0.4)', 'rgba(0,102,255,0.4)', 'rgba(39,26,51,0.4)', 'rgba(153,38,69,0.4)', 'rgba(242,137,121,0.4)', 'rgba(179,158,134,0.4)', 'rgba(43,51,26,0.4)', 'rgba(0,242,194,0.4)',
      'rgba(0,71,179,0.4)', 'rgba(162,0,242,0.4)', 'rgba(255,128,162,0.4)', 'rgba(191,86,48,0.4)', 'rgba(77,68,57,0.4)', 'rgba(166,255,64,0.4)', 'rgba(0,102,82,0.4)', 'rgba(0,20,51,0.4)', 'rgba(102,0,153,0.4)', 'rgba(140,70,89,0.4)',
      'rgba(115,52,29,0.4)', 'rgba(255,170,0,0.4)', 'rgba(225,255,191,0.4)', 'rgba(0,51,48,0.4)', 'rgba(51,71,102,0.4)', 'rgba(138,77,153,0.4)', 'rgba(89,45,57,0.4)', 'rgba(51,23,13,0.4)', 'rgba(153,102,0,0.4)', 'rgba(161,230,115,0.4)',
      'rgba(64,255,242,0.4)', 'rgba(172,195,230,0.4)', 'rgba(167,0,179,0.4)', 'rgba(51,0,7,0.4)', 'rgba(178,113,89,0.4)', 'rgba(64,43,0,0.4)', 'rgba(48,77,38,0.4)', 'rgba(38,153,145,0.4)', 'rgba(0,61,230,0.4)', 'rgba(73,19,77,0.4)',
      'rgba(204,51,71,0.4)', 'rgba(102,65,51,0.4)', 'rgba(102,85,51,0.4)', 'rgba(17,128,0,0.4)', 'rgba(191,255,251,0.4)', 'rgba(121,153,242,0.4)', 'rgba(99,51,102,0.4)', 'rgba(166,124,130,0.4)', 'rgba(191,156,143,0.4)', 'rgba(229,184,0,0.4)',
      'rgba(67,191,48,0.4)', 'rgba(57,218,230,0.4)', 'rgba(32,40,64,0.4)', 'rgba(115,0,107,0.4)', 'rgba(51,38,40,0.4)', 'rgba(255,102,0,0.4)', 'rgba(153,122,0,0.4)', 'rgba(0,51,0,0.4)', 'rgba(70,136,140,0.4)', 'rgba(19,27,77,0.4)',
      'rgba(242,61,230,0.4)', 'rgba(140,56,0,0.4)', 'rgba(89,71,0,0.4)', 'rgba(83,166,83,0.4)', 'rgba(67,88,89,0.4)', 'rgba(57,65,115,0.4)', 'rgba(204,153,201,0.4)', 'rgba(76,31,0,0.4)', 'rgba(255,230,128,0.4)', 'rgba(124,166,124,0.4)',
      'rgba(0,184,230,0.4)', 'rgba(115,115,230,0.4)', 'rgba(230,115,207,0.4)', 'rgba(255,140,64,0.4)', 'rgba(178,161,89,0.4)', 'rgba(0,89,12,0.4)', 'rgba(0,92,115,0.4)', 'rgba(20,0,153,0.4)', 'rgba(166,0,111,0.4)', 'rgba(255,179,128,0.4)',
      'rgba(89,88,67,0.4)', 'rgba(57,115,65,0.4)', 'rgba(19,65,77,0.4)', 'rgba(14,0,102,0.4)', 'rgba(64,0,43,0.4)', 'rgba(178,125,89,0.4)', 'rgba(214,230,0,0.4)', 'rgba(86,115,90,0.4)', 'rgba(0,153,230,0.4)', 'rgba(40,29,115,0.4)', 'rgba(166,83,138)'];

    const labels = [];
    const counts = [];
    const backgroundColor = [];

    let palettePos = Math.floor(Math.random() * palette.length);
    for (const prop in votes) {
        labels.push(prop);
        counts.push(votes[prop]);
        backgroundColor.push(palette[palettePos++]);

        if (palettePos === palette.length) {
          palettePos = 0;
        }
    }

    const data = {
      labels: labels,
      datasets: [{
        label: 'Vote Distribution',
        data: counts,
        backgroundColor: backgroundColor
      }]
    };

    const config = {
      type: 'doughnut',
      data: data,
      options: {
        plugins: {
          legend: {
            display: false
          },
          tooltip: {
            callbacks: {
              label: context =>context.label + ' got ' + context.parsed + ' ' + (context.parsed == 1 ? 'vote' : 'votes')
            }
          }
        }
      }
    };

    new Chart(
      document.getElementById(elementId),
      config
    );
  },

  scrollToBottom: (elementId) => {
    const el = document.getElementById(elementId);
    el.scrollTop = el.scrollHeight - el.clientHeight
  },

  watchScroll: (elementId, dotnetRef, methodName) => {
    const el = document.getElementById(elementId);
    el.addEventListener('scroll', () => {
      // allow 1px inaccuracy by adding 1
      const bottom = el.scrollHeight - el.clientHeight <= el.scrollTop + 1;
      dotnetRef.invokeMethodAsync(methodName,  bottom);
    });
  },

};
