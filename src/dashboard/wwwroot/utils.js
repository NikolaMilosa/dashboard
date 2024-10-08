function createToast(id, content, level) {
    // Create the container div with position-fixed, top-0, end-0, and padding
    const toastContainer = document.createElement('div');
    toastContainer.className = 'position-fixed top-0 end-0 p-3';
    toastContainer.style.zIndex = '11';

    // Create the toast div and set its properties
    const toastId = `toast_${id.replace(/-/g, '')}`;
    const toast = document.createElement('div');
    toast.id = toastId;
    toast.className = `toast align-items-center text-white bg-${level} border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');

    // Create the d-flex container for the toast body and close button
    const toastFlex = document.createElement('div');
    toastFlex.className = 'd-flex';

    // Create the toast body
    const toastBody = document.createElement('div');
    toastBody.className = 'toast-body';
    toastBody.textContent = content; 

    // Create the close button
    const closeButton = document.createElement('button');
    closeButton.className = 'btn-close btn-close-white me-2 m-auto';
    closeButton.setAttribute('data-bs-dismiss', 'toast');
    closeButton.setAttribute('aria-label', 'Close');

    // Append the body and close button to the flex container
    toastFlex.appendChild(toastBody);
    toastFlex.appendChild(closeButton);

    // Append the flex container to the toast
    toast.appendChild(toastFlex);

    // Append the toast to the container
    toastContainer.appendChild(toast);

    // Append the container to the body or any other parent element
    document.body.appendChild(toastContainer);

    // Show the toast using Bootstrap's toast API
    const toastElement = new bootstrap.Toast(toast);
    toastElement.show();
}
