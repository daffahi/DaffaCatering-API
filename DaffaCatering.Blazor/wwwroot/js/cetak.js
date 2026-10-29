export async function cetakElemen(idElemen, judul) {
    const sumber = document.getElementById(idElemen);
    if (!sumber) return;

    const iframe = document.createElement('iframe');
    iframe.setAttribute('aria-hidden', 'true');
    iframe.style.cssText = 'position:fixed;right:0;bottom:0;width:0;height:0;border:0;';
    document.body.appendChild(iframe);

    const gaya = Array.from(document.querySelectorAll('link[rel="stylesheet"], style'))
        .map(n => n.outerHTML)
        .join('');

    const doc = iframe.contentWindow.document;
    doc.open();
    doc.write(
        '<!DOCTYPE html><html><head><meta charset="utf-8">' +
        '<title>' + judul + '</title>' +
        '<base href="' + document.baseURI + '">' +
        gaya +
        '<style>@page{size:A4;margin:12mm 14mm}body{margin:0}</style>' +
        '</head><body>' + sumber.outerHTML + '</body></html>'
    );
    doc.close();

    // tunggu stylesheet dan gambar selesai dimuat sebelum mencetak
    const tunggu = [
        ...doc.querySelectorAll('link[rel="stylesheet"]'),
        ...doc.querySelectorAll('img')
    ].map(el => new Promise(selesai => {
        if (el.complete) return selesai();
        el.addEventListener('load', selesai);
        el.addEventListener('error', selesai);
    }));

    await Promise.race([
        Promise.all(tunggu),
        new Promise(r => setTimeout(r, 4000))
    ]);

    iframe.contentWindow.addEventListener('afterprint', () => iframe.remove());
    setTimeout(() => iframe.remove(), 120000);

    iframe.contentWindow.focus();
    iframe.contentWindow.print();
}